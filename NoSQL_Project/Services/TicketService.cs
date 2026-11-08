// Author: Student2
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services
{
    /// <summary>
    /// Business-logic implementation for incident tickets.
    /// Enforces that new tickets start as Open, calculates deadlines,
    /// builds employee display names by batched lookup, and appends audit log entries.
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository    _ticketRepo;
        private readonly IEmployeeRepository  _empRepo;

        public TicketService(ITicketRepository ticketRepo, IEmployeeRepository empRepo)
        {
            _ticketRepo = ticketRepo;
            _empRepo    = empRepo;
        }

        /// <inheritdoc/>
        public async Task<List<TicketDisplayViewModel>> GetAllForDisplayAsync(string? userId = null)
        {
            var tickets  = await _ticketRepo.GetAllAsync(userId);
            return await EnrichWithNamesAsync(tickets);
        }

        /// <inheritdoc/>
        public Task<Ticket?> GetByIdAsync(string id) => _ticketRepo.GetByIdAsync(id);

        /// <inheritdoc/>
        public async Task<TicketDisplayViewModel?> GetByIdForDisplayAsync(string id)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null) return null;
            var enriched = await EnrichWithNamesAsync([ticket]);
            return enriched.FirstOrDefault();
        }

        /// <inheritdoc/>
        public async Task CreateAsync(CreateTicketViewModel model, string reportedByUserId, string createdByUserId)
        {
            int deadlineDays = model.DeadlineOption switch
            {
                "7"   => 7,
                "14"  => 14,
                "28"  => 28,
                "180" => 180,
                _     => 7
            };

            var ticket = new Ticket
            {
                Title        = model.Title.Trim(),
                Description  = model.Description.Trim(),
                Type         = model.Type,
                Priority     = model.Priority,
                Status       = TicketStatus.Open,          // new tickets always Open
                DateReported = model.DateReported,
                Deadline     = model.DateReported.AddDays(deadlineDays),
                ReportedBy   = model.ReportedByUserId ?? reportedByUserId,
                AssignedTo   = string.IsNullOrWhiteSpace(model.AssignedToUserId) ? null : model.AssignedToUserId,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow,
                Logs         =
                [
                    new TicketLog
                    {
                        Action    = "Ticket created",
                        ChangedBy = createdByUserId,
                        Timestamp = DateTime.UtcNow
                    }
                ]
            };

            await _ticketRepo.AddAsync(ticket);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(EditTicketViewModel model, string changedByUserId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException($"Ticket {model.Id} not found.");

            int deadlineDays = model.DeadlineOption switch
            {
                "7"   => 7,
                "14"  => 14,
                "28"  => 28,
                "180" => 180,
                _     => 7
            };

            // Build audit log entries for every field that changed
            var logs = new List<TicketLog>();

            if (ticket.Status != model.Status)
                logs.Add(MakeLog("Status changed",   ticket.Status,   model.Status,   changedByUserId, model.Note));

            if (ticket.Priority != model.Priority)
                logs.Add(MakeLog("Priority changed", ticket.Priority, model.Priority, changedByUserId, null));

            if (ticket.AssignedTo != model.AssignedToUserId)
                logs.Add(MakeLog("Assignment changed", ticket.AssignedTo ?? "unassigned",
                                 model.AssignedToUserId ?? "unassigned", changedByUserId, model.Note));

            if (ticket.Title != model.Title)
                logs.Add(MakeLog("Title changed", ticket.Title, model.Title, changedByUserId, null));

            if (logs.Count == 0 && !string.IsNullOrWhiteSpace(model.Note))
                logs.Add(MakeLog("Note added", null, null, changedByUserId, model.Note));

            // Apply changes
            ticket.Title       = model.Title.Trim();
            ticket.Description = model.Description.Trim();
            ticket.Type        = model.Type;
            ticket.Priority    = model.Priority;
            ticket.Status      = model.Status;
            ticket.DateReported = model.DateReported;
            ticket.Deadline    = model.DateReported.AddDays(deadlineDays);
            ticket.ReportedBy  = model.ReportedByUserId ?? ticket.ReportedBy;
            ticket.AssignedTo  = string.IsNullOrWhiteSpace(model.AssignedToUserId) ? null : model.AssignedToUserId;
            ticket.UpdatedAt   = DateTime.UtcNow;

            if (model.Status == TicketStatus.Closed || model.Status == TicketStatus.Resolved)
                ticket.ClosedAt ??= DateTime.UtcNow;
            else
                ticket.ClosedAt = null;

            ticket.Logs.AddRange(logs);

            await _ticketRepo.UpdateAsync(ticket);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string id) => _ticketRepo.DeleteAsync(id);

        /// <inheritdoc/>
        public Task<StatusCountsViewModel> GetStatusCountsAsync(string? userId = null)
            => _ticketRepo.GetStatusCountsAsync(userId);

        /// <inheritdoc/>
        public Task<long> GetOpenPastDeadlineCountAsync(string? userId = null)
            => _ticketRepo.GetOpenPastDeadlineCountAsync(userId);

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<List<TicketDisplayViewModel>> EnrichWithNamesAsync(List<Ticket> tickets)
        {
            if (tickets.Count == 0) return [];

            // Collect all unique employee IDs (reporter + assignee)
            var ids = tickets.Select(t => t.ReportedBy)
                .Union(tickets.Where(t => t.AssignedTo != null).Select(t => t.AssignedTo!))
                .Distinct()
                .ToList();

            var employees = await _empRepo.GetByIdsAsync(ids);
            var empDict   = employees.ToDictionary(e => e.Id);

            return tickets.Select(t => new TicketDisplayViewModel
            {
                Ticket        = t,
                ReporterName  = empDict.TryGetValue(t.ReportedBy, out var r)  ? r.Name  : "Unknown",
                ReporterEmail = empDict.TryGetValue(t.ReportedBy, out var r2) ? r2.Email : string.Empty,
                AssigneeName  = t.AssignedTo != null && empDict.TryGetValue(t.AssignedTo, out var a) ? a.Name : null
            }).ToList();
        }

        private static TicketLog MakeLog(string action, string? prev, string? next,
                                          string changedBy, string? note) => new()
        {
            Action        = action,
            PreviousValue = prev,
            NewValue      = next,
            Note          = note,
            ChangedBy     = changedBy,
            Timestamp     = DateTime.UtcNow
        };
    }
}
