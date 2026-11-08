// Author: Student2
namespace NoSQL_Project.Models
{
    /// <summary>Allowed values for Ticket.Priority.</summary>
    public static class TicketPriority
    {
        public const string Low    = "Low";
        public const string Normal = "Normal";
        public const string High   = "High";

        public static readonly string[] All = [Low, Normal, High];

        /// <summary>Numeric weight for sort-by-priority (High > Normal > Low).</summary>
        public static int Weight(string priority) => priority switch
        {
            High   => 3,
            Normal => 2,
            Low    => 1,
            _      => 0
        };
    }
}
