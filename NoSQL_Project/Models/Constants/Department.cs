// Author: Student1
namespace NoSQL_Project.Models
{
    /// <summary>Allowed department/branch values for The Garden Group, matching ERD "department" field.</summary>
    public static class Department
    {
        public const string Haarlem      = "Haarlem";
        public const string Amsterdam    = "Amsterdam";
        public const string Knuppeldam   = "Knuppeldam";
        public const string Headquarters = "Headquarters";

        public static readonly string[] All =
            [Haarlem, Amsterdam, Knuppeldam, Headquarters];
    }
}
