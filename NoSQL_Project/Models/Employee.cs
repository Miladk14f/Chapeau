// Author: Student1
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    /// <summary>
    /// Represents a NoDesk system user (Employee or ServiceDesk agent).
    /// Stored in the "employees" collection in MongoDB.
    /// Maps directly to the EMPLOYEES entity in the ERD.
    /// </summary>
    public class Employee
    {
        /// <summary>MongoDB document identifier. BsonRepresentation maps ObjectId → C# string.</summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        /// <summary>Full display name (e.g. "John Doe").</summary>
        public string Name { get; set; } = default!;

        /// <summary>Unique e-mail address; used as login credential and display identifier.</summary>
        public string Email { get; set; } = default!;

        /// <summary>PBKDF2-SHA256 hash produced by <see cref="Helpers.PasswordHelper"/>.</summary>
        public string PasswordHash { get; set; } = default!;

        /// <summary>Role: "Employee" or "ServiceDesk". See <see cref="UserRole"/>.</summary>
        public string Role { get; set; } = default!;

        /// <summary>Office department / branch. See <see cref="Department"/>.</summary>
        public string DepartmentName { get; set; } = default!;

        /// <summary>UTC timestamp when the account was created.</summary>
        public DateTime CreatedAt { get; set; }
    }
}
