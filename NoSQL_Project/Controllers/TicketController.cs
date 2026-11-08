// Author: Student2 + Student4 (Search action)
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    /// <summary>
    /// CRUD for incident tickets.
    /// Regular employees can create and view their own tickets.
    /// ServiceDesk can create, view, edit and delete all tickets.
    /// </summary>
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketService       _ticketService;
        private readonly IEmployeeService     _employeeService;
        private readonly ITicketSearchService _searchService;

        public TicketController(ITicketService ticketService,
                                IEmployeeService employeeService,
                                ITicketSearchService searchService)
        {
            _ticketService   = ticketService;
            _employeeService = employeeService;
            _searchService   = searchService;
        }

        // GET /Ticket
        public async Task<IActionResult> Index()
        {
            try
            {
                bool   isServiceDesk = User.IsInRole(UserRole.ServiceDesk);
                string? userId       = isServiceDesk ? null : CurrentUserId();

                var tickets = await _ticketService.GetAllForDisplayAsync(userId);
                return View(tickets);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not load tickets: " + ex.Message;
                return View(new List<TicketDisplayViewModel>());
            }
        }

        // GET /Ticket/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var vm = await _ticketService.GetByIdForDisplayAsync(id);
                if (vm == null) return NotFound();

                // Regular employee can only see own tickets
                if (!User.IsInRole(UserRole.ServiceDesk) && vm.Ticket.ReportedBy != CurrentUserId())
                    return Forbid();

                // Load changedBy names for log entries
                var logEmployeeIds = vm.Ticket.Logs.Select(l => l.ChangedBy).Distinct().ToList();
                var logEmployees   = await _employeeService.GetByIdsAsync(logEmployeeIds);
                var logEmpDict     = logEmployees?.ToDictionary(e => e.Id, e => e.Name)
                                     ?? new Dictionary<string, string>();
                ViewBag.LogEmployeeNames = logEmpDict;

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not load ticket: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET /Ticket/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            var vm = new CreateTicketViewModel { DateReported = DateTime.Now };
            return View(vm);
        }

        // POST /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(model);
            }

            try
            {
                string userId = CurrentUserId()!;
                await _ticketService.CreateAsync(model, userId, userId);
                TempData["Success"] = "Ticket created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not create ticket: " + ex.Message);
                await PopulateDropdownsAsync();
                return View(model);
            }
        }

        // GET /Ticket/Edit/{id}
        [Authorize(Roles = "ServiceDesk")]
        public async Task<IActionResult> Edit(string id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null) return NotFound();

            var vm = new EditTicketViewModel
            {
                Id              = ticket.Id,
                Title           = ticket.Title,
                Description     = ticket.Description,
                Type            = ticket.Type,
                Priority        = ticket.Priority,
                Status          = ticket.Status,
                DateReported    = ticket.DateReported,
                ReportedByUserId = ticket.ReportedBy,
                AssignedToUserId = ticket.AssignedTo,
                CurrentDeadline  = ticket.Deadline,
                // Map deadline back to option string
                DeadlineOption   = MapDeadlineToDays(ticket.DateReported, ticket.Deadline)
            };

            await PopulateDropdownsAsync();
            return View(vm);
        }

        // POST /Ticket/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ServiceDesk")]
        public async Task<IActionResult> Edit(EditTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(model);
            }

            try
            {
                await _ticketService.UpdateAsync(model, CurrentUserId()!);
                TempData["Success"] = "Ticket updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not update ticket: " + ex.Message);
                await PopulateDropdownsAsync();
                return View(model);
            }
        }

        // GET /Ticket/Delete/{id}  — confirmation page
        [Authorize(Roles = "ServiceDesk")]
        public async Task<IActionResult> Delete(string id)
        {
            var vm = await _ticketService.GetByIdForDisplayAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // POST /Ticket/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ServiceDesk")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _ticketService.DeleteAsync(id);
                TempData["Success"] = "Ticket deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not delete ticket: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET /Ticket/Search  — Individual feature
        [HttpGet]
        public async Task<IActionResult> Search(string? q, string mode = "and")
        {
            if (string.IsNullOrWhiteSpace(q))
                return View(new List<TicketDisplayViewModel>());

            try
            {
                bool   isServiceDesk = User.IsInRole(UserRole.ServiceDesk);
                string? userId       = isServiceDesk ? null : CurrentUserId();
                bool   useAnd        = mode != "or";

                var results = await _searchService.SearchAsync(q, useAnd, userId);
                ViewBag.Query = q;
                ViewBag.Mode  = mode;
                return View(results);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Search failed: " + ex.Message;
                return View(new List<TicketDisplayViewModel>());
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private string? CurrentUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Types     = TicketType.All.Select(t => new SelectListItem(t, t));
            ViewBag.Priorities = TicketPriority.All.Select(p => new SelectListItem(p, p));
            ViewBag.Statuses  = TicketStatus.All.Select(s => new SelectListItem(s, s));
            ViewBag.Deadlines = new[]
            {
                new SelectListItem("7 days",   "7"),
                new SelectListItem("14 days",  "14"),
                new SelectListItem("28 days",  "28"),
                new SelectListItem("6 months", "180")
            };

            if (User.IsInRole(UserRole.ServiceDesk))
            {
                var employees = await _employeeService.GetAllAsync();
                ViewBag.Employees = employees
                    .Select(e => new SelectListItem(e.Name, e.Id));
            }
        }

        private static string MapDeadlineToDays(DateTime reported, DateTime deadline)
        {
            int days = (int)(deadline - reported).TotalDays;
            return days switch
            {
                <= 7   => "7",
                <= 14  => "14",
                <= 28  => "28",
                _      => "180"
            };
        }
    }
}
