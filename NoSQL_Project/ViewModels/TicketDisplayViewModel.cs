// Author: Student3
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    /// <summary>
    /// Combines a Ticket with the display names of the reporter and optional assignee.
    /// Used for ticket list and detail views. The names are resolved from a batched
    /// employee lookup in the service layer, avoiding a per-ticket query.
    /// </summary>
    public class TicketDisplayViewModel
    {
        /// <summary>The underlying ticket document.</summary>
        public Ticket Ticket { get; set; } = default!;

        /// <summary>Full name of the employee who reported the ticket.</summary>
        public string ReporterName { get; set; } = "Unknown";

        /// <summary>E-mail of the reporting employee (used for filter-by-email).</summary>
        public string ReporterEmail { get; set; } = string.Empty;

        /// <summary>Full name of the assigned agent; null if unassigned.</summary>
        public string? AssigneeName { get; set; }
    }
}
