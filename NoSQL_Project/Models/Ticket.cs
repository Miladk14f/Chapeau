// Author: Student2
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    /// <summary>
    /// Represents a service-desk incident ticket.
    /// Stored in the "tickets" collection in MongoDB.
    /// Maps to the TICKETS entity in the ERD; LOG entries are embedded.
    /// </summary>
    public class Ticket
    {
        /// <summary>MongoDB document identifier.</summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        /// <summary>Short title / subject of the incident.</summary>
        public string Title { get; set; } = default!;

        /// <summary>Full description of the problem.</summary>
        public string Description { get; set; } = default!;

        /// <summary>Current status. One of <see cref="TicketStatus"/> constants.</summary>
        public string Status { get; set; } = default!;

        /// <summary>Priority level. One of <see cref="TicketPriority"/> constants.</summary>
        public string Priority { get; set; } = default!;

        /// <summary>
        /// Incident type (Software | Hardware | Service).
        /// Not in ERD but required by functional spec; added as an extension field.
        /// </summary>
        public string Type { get; set; } = default!;

        /// <summary>
        /// FK → employees._id.  Stored as ObjectId string.
        /// For list display, the application loads employee names separately via
        /// a $lookup aggregation or a batched GetByIds call.
        /// Design alternative: embed a reporter snapshot (extended reference pattern).
        /// We chose pure FK to stay faithful to the ERD and avoid duplicate-data
        /// update overhead on employee name changes.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReportedBy { get; set; } = default!;

        /// <summary>
        /// FK → employees._id for the assigned service-desk agent.
        /// Nullable — tickets start unassigned (null).
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AssignedTo { get; set; }

        /// <summary>When the incident was reported by the end-user.</summary>
        public DateTime DateReported { get; set; }

        /// <summary>
        /// Calculated deadline date. Stored as an absolute DateTime so the query
        /// <c>deadline &lt; DateTime.UtcNow</c> works without arithmetic at query time.
        /// </summary>
        public DateTime Deadline { get; set; }

        /// <summary>UTC document creation timestamp.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>UTC timestamp of last modification.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>UTC timestamp when the ticket was closed; null while still open.</summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// Embedded audit log — every status change, assignment, or field edit
        /// appends an entry here. See LOG entity in ERD.
        /// Never queried independently → embedding is correct (no $lookup cost).
        /// </summary>
        public List<TicketLog> Logs { get; set; } = new();

        /// <summary>Computed: ticket is open and past its deadline.</summary>
        [BsonIgnore]
        public bool IsPastDeadline => Status == TicketStatus.Open && Deadline < DateTime.UtcNow;
    }
}
