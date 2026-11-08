// Author: Student2
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="ITicketRepository"/>.
    /// Operates on the "tickets" collection.
    /// Strongly-typed Builders&lt;T&gt; API for filters, sorts and updates.
    /// </summary>
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase database)
        {
            _tickets = database.GetCollection<Ticket>("tickets");
        }

        /// <inheritdoc/>
        public async Task<List<Ticket>> GetAllAsync(string? userId = null)
        {
            var filter = userId == null
                ? Builders<Ticket>.Filter.Empty
                : Builders<Ticket>.Filter.Eq(t => t.ReportedBy, userId);

            return await _tickets
                .Find(filter)
                .SortByDescending(t => t.DateReported)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Ticket?> GetByIdAsync(string id)
        {
            return await _tickets
                .Find(Builders<Ticket>.Filter.Eq(t => t.Id, id))
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task AddAsync(Ticket ticket)
        {
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _tickets.InsertOneAsync(ticket);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Ticket ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticket.Id);
            await _tickets.ReplaceOneAsync(filter, ticket);
        }

        /// <inheritdoc/>
        public async Task UpdateStatusAsync(string id, string status)
        {
            var now    = DateTime.UtcNow;
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);

            var updateDef = Builders<Ticket>.Update
                .Set(t => t.Status,    status)
                .Set(t => t.UpdatedAt, now);

            if (status == TicketStatus.Closed || status == TicketStatus.Resolved)
                updateDef = updateDef.Set(t => t.ClosedAt, now);

            await _tickets.UpdateOneAsync(filter, updateDef);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);
            await _tickets.DeleteOneAsync(filter);
        }

        /// <inheritdoc/>
        public async Task<StatusCountsViewModel> GetStatusCountsAsync(string? userId = null)
        {
            // Single $group pipeline — one round trip returns all three counts.
            // Alternative: three separate CountDocumentsAsync calls; easier to read but
            // three times the network overhead.  $group wins here.
            var matchFilter = userId == null
                ? Builders<Ticket>.Filter.Empty
                : Builders<Ticket>.Filter.Eq(t => t.ReportedBy, userId);

            var pipeline = _tickets
                .Aggregate()
                .Match(matchFilter)
                .Group(t => t.Status, g => new { Status = g.Key, Count = g.Count() });

            var groups = await pipeline.ToListAsync();

            return new StatusCountsViewModel
            {
                Open     = groups.FirstOrDefault(g => g.Status == TicketStatus.Open)    ?.Count ?? 0,
                Resolved = groups.FirstOrDefault(g => g.Status == TicketStatus.Resolved)?.Count ?? 0,
                Closed   = groups.FirstOrDefault(g => g.Status == TicketStatus.Closed)  ?.Count ?? 0
            };
        }

        /// <inheritdoc/>
        public async Task<long> GetOpenPastDeadlineCountAsync(string? userId = null)
        {
            var now = DateTime.UtcNow;

            var filter = Builders<Ticket>.Filter.And(
                Builders<Ticket>.Filter.Eq(t => t.Status,   TicketStatus.Open),
                Builders<Ticket>.Filter.Lt(t => t.Deadline, now));

            if (userId != null)
                filter = Builders<Ticket>.Filter.And(
                    filter,
                    Builders<Ticket>.Filter.Eq(t => t.ReportedBy, userId));

            return await _tickets.CountDocumentsAsync(filter);
        }

        /// <inheritdoc/>
        public async Task<List<Ticket>> SearchAsync(string query, string? userId = null)
        {
            // MongoDB text index on { subject: "text", description: "text" } must be created
            // at startup (see DatabaseInitializer).  Text search supports AND (default, all words)
            // and OR ("word1 word2" with no quotes in the query string uses OR).
            // We pass the raw query to $text; the calling service splits and wraps for AND mode.
            //
            // Alternative: regex search per-field — no index required, but full collection scan
            // and no relevance scoring.  Text index is better for real search use cases.
            var textFilter = Builders<Ticket>.Filter.Text(query);

            if (userId != null)
                textFilter = Builders<Ticket>.Filter.And(
                    textFilter,
                    Builders<Ticket>.Filter.Eq(t => t.ReportedBy, userId));

            // Sort by text relevance score first, then by dateReported descending
            var scoreProjection = Builders<Ticket>.Projection
                .MetaTextScore("score");

            return await _tickets
                .Find(textFilter)
                .Sort(Builders<Ticket>.Sort
                    .MetaTextScore("score")
                    .Descending(t => t.DateReported))
                .ToListAsync();
        }
    }
}
