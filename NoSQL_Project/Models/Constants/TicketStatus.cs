// Author: Student2
namespace NoSQL_Project.Models
{
    /// <summary>Allowed values for Ticket.Status.</summary>
    public static class TicketStatus
    {
        public const string Open     = "Open";
        public const string Resolved = "Resolved";
        public const string Closed   = "Closed";

        public static readonly string[] All = [Open, Resolved, Closed];
    }
}
