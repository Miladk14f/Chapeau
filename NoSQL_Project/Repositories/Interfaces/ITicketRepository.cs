// Author: Student2
using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories.Interfaces
{
    /// <summary>
    /// Contract for all ticket data access operations against the "tickets" collection.
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
        /// Returns all tickets, optionally filtered to those reported by a specific user,
        /// sorted by dateReported descending (most recent first).
        /// </summary>
        /// <param name="userId">When non-null, filters to tickets whose reportedBy equals this id.</param>
        /// <returns>List of matching Ticket documents.</returns>
        Task<List<Ticket>> GetAllAsync(string? userId = null);

        /// <summary>Returns a single ticket by its ObjectId string.</summary>
        /// <param name="id">MongoDB ObjectId as string.</param>
        /// <returns>The matching Ticket, or <c>null</c> if not found.</returns>
        Task<Ticket?> GetByIdAsync(string id);

        /// <summary>Inserts a new ticket document.</summary>
        /// <param name="ticket">Fully populated Ticket; Id is assigned by MongoDB.</param>
        Task AddAsync(Ticket ticket);

        /// <summary>Replaces the entire ticket document identified by ticket.Id.</summary>
        /// <param name="ticket">Updated Ticket document.</param>
        Task UpdateAsync(Ticket ticket);

        /// <summary>Updates only the Status field (and sets UpdatedAt / ClosedAt).</summary>
        /// <param name="id">Ticket ObjectId.</param>
        /// <param name="status">New status value from <see cref="TicketStatus"/>.</param>
        Task UpdateStatusAsync(string id, string status);

        /// <summary>Permanently deletes a ticket. ServiceDesk only.</summary>
        /// <param name="id">Ticket ObjectId.</param>
        Task DeleteAsync(string id);

        /// <summary>
        /// Returns aggregated Open/Resolved/Closed counts in a single $group pipeline,
        /// optionally scoped to one user's tickets.
        /// One pipeline round trip replaces three separate CountDocuments calls —
        /// better latency for the dashboard.
        /// Alternative: three separate CountDocuments with filter — simpler code,
        /// three round trips; acceptable for small data but we prefer the single pipeline.
        /// </summary>
        /// <param name="userId">When non-null, counts only this user's tickets.</param>
        /// <returns>Status count totals ready for the dashboard view.</returns>
        Task<StatusCountsViewModel> GetStatusCountsAsync(string? userId = null);

        /// <summary>
        /// Counts tickets that are Open and whose deadline has already passed.
        /// Uses CountDocumentsAsync with a compound filter (status=Open AND deadline &lt; now).
        /// </summary>
        /// <param name="userId">When non-null, restricts to this user's tickets.</param>
        /// <returns>Count of overdue open tickets.</returns>
        Task<long> GetOpenPastDeadlineCountAsync(string? userId = null);

        /// <summary>
        /// Text-search tickets whose subject or description contains the query words.
        /// Uses MongoDB text index; AND mode requires all words, OR mode requires any word.
        /// </summary>
        /// <param name="query">Search phrase entered by the user.</param>
        /// <param name="userId">When non-null, restricts search to this user's tickets.</param>
        /// <returns>Matching tickets sorted by relevance score, then dateReported desc.</returns>
        Task<List<Ticket>> SearchAsync(string query, string? userId = null);
    }
}
