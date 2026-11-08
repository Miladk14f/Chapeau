// Author: Student1
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    /// <summary>
    /// User management — ServiceDesk only.
    /// Full CRUD for employee accounts.
    /// </summary>
    [Authorize(Roles = "ServiceDesk")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
            => _employeeService = employeeService;

        // GET /Employee
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _employeeService.GetAllWithTicketCountAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not load employees: " + ex.Message;
                return View(new List<UserWithTicketCountViewModel>());
            }
        }

        // GET /Employee/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new CreateEmployeeViewModel());
        }

        // POST /Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            try
            {
                await _employeeService.CreateAsync(model);
                TempData["Success"] = $"User '{model.Name}' created.";

                if (model.SendPassword)
                    TempData["Info"] = $"Temporary password: {model.Password} — " +
                                       "In production this would be e-mailed to the user.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not create user: " + ex.Message);
                PopulateDropdowns();
                return View(model);
            }
        }

        // GET /Employee/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var emp = await _employeeService.GetByIdAsync(id);
            if (emp == null) return NotFound();

            var vm = new EditEmployeeViewModel
            {
                Id             = emp.Id,
                Name           = emp.Name,
                Email          = emp.Email,
                Role           = emp.Role,
                DepartmentName = emp.DepartmentName
            };

            PopulateDropdowns();
            return View(vm);
        }

        // POST /Employee/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(model);
            }

            try
            {
                await _employeeService.UpdateAsync(model);
                TempData["Success"] = "User updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Could not update user: " + ex.Message);
                PopulateDropdowns();
                return View(model);
            }
        }

        // GET /Employee/Delete/{id}  — confirmation page
        public async Task<IActionResult> Delete(string id)
        {
            var emp = await _employeeService.GetByIdAsync(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        // POST /Employee/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _employeeService.DeleteAsync(id);
                TempData["Success"] = "User deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not delete user: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──────────────────────────────────────────────────────────────
        private void PopulateDropdowns()
        {
            ViewBag.Roles       = new[] { UserRole.Employee, UserRole.ServiceDesk }
                                    .Select(r => new SelectListItem(r, r));
            ViewBag.Departments = Department.All
                                    .Select(d => new SelectListItem(d, d));
        }
    }
}
