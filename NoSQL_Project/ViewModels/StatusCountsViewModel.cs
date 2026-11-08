// Author: Student3
namespace NoSQL_Project.ViewModels
{
    /// <summary>
    /// Aggregated ticket status counts returned by a single MongoDB aggregation pipeline.
    /// Used on the dashboard for both Employee (own tickets) and ServiceDesk (all tickets).
    /// </summary>
    public class StatusCountsViewModel
    {
        /// <summary>Number of tickets with status Open.</summary>
        public int Open { get; set; }

        /// <summary>Number of tickets with status Resolved.</summary>
        public int Resolved { get; set; }

        /// <summary>Number of tickets with status Closed.</summary>
        public int Closed { get; set; }

        /// <summary>Total ticket count across all statuses.</summary>
        public int Total => Open + Resolved + Closed;

        /// <summary>Percentage of open tickets; 0 when Total is 0 (divide-by-zero guard).</summary>
        public double OpenPercent     => Total == 0 ? 0 : Math.Round(Open     * 100.0 / Total, 1);

        /// <summary>Percentage of resolved tickets.</summary>
        public double ResolvedPercent => Total == 0 ? 0 : Math.Round(Resolved * 100.0 / Total, 1);

        /// <summary>Percentage of closed (without resolve) tickets.</summary>
        public double ClosedPercent   => Total == 0 ? 0 : Math.Round(Closed   * 100.0 / Total, 1);
    }
}
