// Author: Student4  (Individual feature — Ticket transfer with history)
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    /// <summary>
    /// Audit-log entry embedded inside a Ticket document.
    /// Maps to the LOG entity in the ERD.
    ///
    /// Design choice: logs are embedded (not a separate collection) because:
    ///   - They are always read together with their parent ticket.
    ///   - They are never queried independently.
    ///   - Embedding avoids a $lookup on every ticket-detail read.
    ///   - Max ~50 log entries per ticket keeps documents well under MongoDB's 16 MB limit.
    /// </summary>
    public class TicketLog
    {
        /// <summary>Describes what changed, e.g. "Status changed", "Assigned to", "Priority updated".</summary>
        public string Action { get; set; } = default!;

        /// <summary>Field value before the change; null for create events.</summary>
        public string? PreviousValue { get; set; }

        /// <summary>Field value after the change; null for delete/close events.</summary>
        public string? NewValue { get; set; }

        /// <summary>Optional free-text note added by the agent at the time of the change.</summary>
        public string? Note { get; set; }

        /// <summary>ObjectId reference to the employee who made the change.</summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string ChangedBy { get; set; } = default!;

        /// <summary>UTC timestamp of the change.</summary>
        public DateTime Timestamp { get; set; }
    }
}
