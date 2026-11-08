// Author: Student1
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Models;

namespace NoSQL_Project.Helpers
{
    /// <summary>
    /// Creates all required MongoDB indexes once at application startup.
    /// Called from Program.cs inside a scoped service scope to get IMongoDatabase.
    /// Indexes are idempotent — CreateMany is a no-op if the index already exists
    /// with the same definition, so this is safe to run on every restart.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static async Task CreateIndexesAsync(IMongoDatabase database)
        {
            // ── employees ─────────────────────────────────────────────────────────
            var employees = database.GetCollection<Employee>("employees");

            await employees.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<Employee>(
                    Builders<Employee>.IndexKeys.Ascending(e => e.Email),
                    new CreateIndexOptions { Unique = true, Name = "email_unique" }),
            ]);

            // ── tickets ───────────────────────────────────────────────────────────
            var tickets = database.GetCollection<Ticket>("tickets");

            await tickets.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<Ticket>(
                    Builders<Ticket>.IndexKeys.Ascending(t => t.ReportedBy),
                    new CreateIndexOptions { Name = "ticket_reportedBy_idx" }),

                new CreateIndexModel<Ticket>(
                    Builders<Ticket>.IndexKeys.Ascending(t => t.Status),
                    new CreateIndexOptions { Name = "ticket_status_idx" }),

                new CreateIndexModel<Ticket>(
                    Builders<Ticket>.IndexKeys.Descending(t => t.DateReported),
                    new CreateIndexOptions { Name = "ticket_dateReported_idx" }),

                // Compound text index for the search individual feature
                new CreateIndexModel<Ticket>(
                    Builders<Ticket>.IndexKeys
                        .Text(t => t.Title)
                        .Text(t => t.Description),
                    new CreateIndexOptions { Name = "ticket_text_search" })
            ]);
        }
    }
}
