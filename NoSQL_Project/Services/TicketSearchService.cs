// Individual feature — Author: Student4
// Full-text keyword search for tickets using a MongoDB text index on title + description.
// Supports AND mode (all words must appear) and OR mode (any word suffices).

using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services
{
    /// <summary>
    /// Individual feature: searches tickets by keywords using MongoDB's text index.
    ///
    /// Design rationale:
    ///   MongoDB $text with a compound index on title + description is used instead
    ///   of regex because:
    ///   - Text index supports relevance scoring ($meta textScore) so most relevant
    ///     results appear first.
    ///   - $text uses an inverted index — O(1) lookup vs O(n) full scan for regex.
    ///   - Regex alternative: Builders.Filter.Regex(t => t.Title, pattern) — works without
    ///     an index but scans every document; fine for &lt;1,000 tickets, slow at scale.
    ///   - $text does not support partial-word matching by default; for that, Atlas Search
    ///     (Lucene) would be the next step.
    /// </summary>
    public class TicketSearchService : ITicketSearchService
    {
        private readonly ITicketRepository   _ticketRepo;
        private readonly IEmployeeRepository _empRepo;

        public TicketSearchService(ITicketRepository ticketRepo, IEmployeeRepository empRepo)
        {
            _ticketRepo = ticketRepo;
            _empRepo    = empRepo;
        }

        /// <inheritdoc/>
        public async Task<List<TicketDisplayViewModel>> SearchAsync(
            string query, bool useAndMode = true, string? userId = null)
        {
            if (string.IsNullOrWhiteSpace(query)) return [];

            // AND mode: wrap each word in quotes so MongoDB requires all words.
            // e.g. "vpn printer" becomes '"vpn" "printer"'
            // OR mode: pass the raw query; MongoDB $text treats space-separated words as OR.
            string mongoQuery = useAndMode
                ? string.Join(" ", query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(w => $"\"{w}\""))
                : query;

            var tickets = await _ticketRepo.SearchAsync(mongoQuery, userId);

            if (tickets.Count == 0) return [];

            // Batch-load employee names for the found tickets
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
    }
}
