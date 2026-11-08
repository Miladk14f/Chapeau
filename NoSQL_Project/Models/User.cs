// Author: Student1
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    /// <summary>
    /// Represents a NoDesk system user (Employee or ServiceDesk agent).
    /// Stored in the "users" collection in MongoDB.
    /// </summary>
    public class User
    {
        /// <summary>MongoDB document identifier. BsonRepresentation maps the ObjectId to a C# string.</summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        /// <summary>User's first name.</summary>
        public string FirstName { get; set; } = default!;

        /// <summary>User's last name.</summary>
        public string LastName { get; set; } = default!;

        /// <summary>Unique login username.</summary>
        public string Username { get; set; } = default!;

        /// <summary>Unique e-mail address. Used as contact and display identifier.</summary>
        public string Email { get; set; } = default!;

        /// <summary>Dutch mobile phone number.</summary>
        public string PhoneNumber { get; set; } = default!;

        /// <summary>Office location. One of <see cref="UserLocation"/> constants.</summary>
        public string Location { get; set; } = default!;

        /// <summary>Role: <c>Employee</c> or <c>ServiceDesk</c>. See <see cref="UserRole"/>.</summary>
        public string Role { get; set; } = default!;

        /// <summary>PBKDF2-SHA256 password hash produced by <see cref="Helpers.PasswordHelper"/>.</summary>
        public string PasswordHash { get; set; } = default!;

        /// <summary>UTC timestamp when the account was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Computed display name.</summary>
        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
