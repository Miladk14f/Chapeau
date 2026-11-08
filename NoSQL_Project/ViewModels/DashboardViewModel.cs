// Author: Student3
namespace NoSQL_Project.ViewModels
{
    /// <summary>Data model for the shared Dashboard view (used by both roles).</summary>
    public class DashboardViewModel
    {
        /// <summary>Ticket status breakdown for this user's scope.</summary>
        public StatusCountsViewModel StatusCounts { get; set; } = new();

        /// <summary>Number of open tickets whose deadline has passed.</summary>
        public long PastDeadlineCount { get; set; }
    }
}
