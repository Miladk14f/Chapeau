// Author: Student2
namespace NoSQL_Project.Models
{
    /// <summary>Allowed values for Ticket.Type.</summary>
    public static class TicketType
    {
        public const string Software = "Software";
        public const string Hardware = "Hardware";
        public const string Service  = "Service";

        public static readonly string[] All = [Software, Hardware, Service];
    }
}
