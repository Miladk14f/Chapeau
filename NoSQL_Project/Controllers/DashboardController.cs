// Author: Student3
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    /// <summary>
    /// Displays the dashboard statistics page.
    /// Both roles use the same view; the data scope differs:
    ///   - Employee: own tickets only.
    ///   - ServiceDesk: all tickets.
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITicketService _ticketService;

        public DashboardController(ITicketService ticketService)
            => _ticketService = ticketService;

        // GET /Dashboard
        public async Task<IActionResult> Index()
        {
            try
            {
                // Determine scope based on role
                bool isServiceDesk = User.IsInRole(UserRole.ServiceDesk);
                string? userId     = isServiceDesk
                    ? null
                    : User.FindFirstValue(ClaimTypes.NameIdentifier);

                var statusCounts   = await _ticketService.GetStatusCountsAsync(userId);
                var pastDeadline   = await _ticketService.GetOpenPastDeadlineCountAsync(userId);

                var vm = new DashboardViewModel
                {
                    StatusCounts     = statusCounts,
                    PastDeadlineCount = pastDeadline
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not load dashboard data. " + ex.Message;
                return View(new DashboardViewModel());
            }
        }
    }
}
