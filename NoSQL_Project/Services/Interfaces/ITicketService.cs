// Author: Student2
using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    /// <summary>Business-logic layer for incident tickets.</summary>
    public interface ITicketService
    {
        /// <summary>
        /// Returns tickets for display (with reporter/assignee names resolved),
        /// optionally scoped to a single user's tickets.
        /// </summary>
        Task<List<TicketDisplayViewModel>> GetAllForDisplayAsync(string? userId = null);

        /// <summary>Returns a single ticket by id, or null.</summary>
        Task<Ticket?> GetByIdAsync(string id);

        /// <summary>Returns a single ticket enriched with employee display names, or null.</summary>
        Task<TicketDisplayViewModel?> GetByIdForDisplayAsync(string id);

        /// <summary>
        /// Creates a new ticket. Status is forced to Open regardless of model input.
        /// Deadline is calculated from DeadlineOption days added to DateReported.
        /// </summary>
        Task CreateAsync(CreateTicketViewModel model, string reportedByUserId, string createdByUserId);

        /// <summary>
        /// Updates an existing ticket. Appends a TicketLog entry for every changed field.
        /// </summary>
        Task UpdateAsync(EditTicketViewModel model, string changedByUserId);

        /// <summary>Deletes a ticket permanently.</summary>
        Task DeleteAsync(string id);

        /// <summary>Returns Open/Resolved/Closed counts for the dashboard.</summary>
        Task<StatusCountsViewModel> GetStatusCountsAsync(string? userId = null);

        /// <summary>Returns the count of open tickets past their deadline.</summary>
        Task<long> GetOpenPastDeadlineCountAsync(string? userId = null);
    }
}
