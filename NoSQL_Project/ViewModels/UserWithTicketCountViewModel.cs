// Author: Student3
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    /// <summary>
    /// Pairs an Employee with the total number of tickets they reported.
    /// Returned by the $lookup aggregation in EmployeeRepository.
    /// </summary>
    public class UserWithTicketCountViewModel
    {
        /// <summary>The employee document.</summary>
        public Employee Employee { get; set; } = default!;

        /// <summary>Total tickets reported by this employee.</summary>
        public int TicketCount { get; set; }
    }
}
