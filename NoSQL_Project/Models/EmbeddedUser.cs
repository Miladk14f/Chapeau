// Author: Student1
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    /// <summary>
    /// Lightweight user snapshot embedded inside a Ticket document.
    /// Uses the "extended reference pattern": store the foreign key (UserId)
    /// plus the fields most often needed when displaying a ticket list,
    /// so ticket list pages never need a $lookup / JOIN.
    /// </summary>
    public class EmbeddedUser
    {
        /// <summary>Reference back to users._id.</summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = default!;

        /// <summary>Snapshot of firstName at time of ticket creation/last update.</summary>
        public string FirstName { get; set; } = default!;

        /// <summary>Snapshot of lastName.</summary>
        public string LastName { get; set; } = default!;

        /// <summary>Snapshot of email.</summary>
        public string Email { get; set; } = default!;

        /// <summary>Computed display name from embedded fields.</summary>
        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
