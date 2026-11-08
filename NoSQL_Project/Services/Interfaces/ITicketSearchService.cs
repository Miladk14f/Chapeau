// Individual feature — Author: Student4
// User story: As any employee, I want to search for tickets by typing keywords so that
// I can quickly find relevant incidents without scrolling through the full list.
// Acceptance criteria: AND mode (all words must match), OR mode (any word matches),
// results sorted by relevance then by date descending, scoped by role.

using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    /// <summary>
    /// Individual feature: full-text search for tickets.
    /// Backed by a MongoDB compound text index on title + description.
    /// </summary>
    public interface ITicketSearchService
    {
        /// <summary>
        /// Searches tickets whose title or description matches the query.
        /// </summary>
        /// <param name="query">Words typed by the user.</param>
        /// <param name="useAndMode">
        ///   When <c>true</c> (default), all words must appear (wraps each word in quotes for MongoDB $text AND).
        ///   When <c>false</c>, any word suffices (plain space-separated query = MongoDB $text OR).
        /// </param>
        /// <param name="userId">When non-null, restricts results to this user's tickets (Employee role).</param>
        /// <returns>Matching tickets with employee names, sorted by relevance then date desc.</returns>
        Task<List<TicketDisplayViewModel>> SearchAsync(string query, bool useAndMode = true, string? userId = null);
    }
}
