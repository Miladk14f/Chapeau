using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly IStaffService _staffService;

        public ManagerController(IManagerService managerService, IStaffService staffService)
        {
            _managerService = managerService;
            _staffService = staffService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("StaffRole") != "manager")
                context.Result = RedirectToAction("Login", "Staff");

            base.OnActionExecuting(context);
        }

        public IActionResult Index(string tab = "revenue", string stockFilter = "all")
        {
            return View(_managerService.GetDashboard(tab, stockFilter));
        }

        [HttpGet]
        public IActionResult StaffManagement()
        {
            return View(_staffService.GetAllStaff());
        }

        [HttpPost]
        public IActionResult CreateStaff(string name, string role, string pin)
        {
            _staffService.CreateStaff(name, role, pin);
            return RedirectToAction("StaffManagement");
        }

        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            Staff staff = _staffService.GetStaffById(id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }

        [HttpPost]
        public IActionResult UpdateStaff(int id, string name, string role, string pin)
        {
            if (!_staffService.UpdateStaffDetails(id, name, role, pin))
                return NotFound();

            return RedirectToAction("StaffManagement");
        }

        [HttpPost]
        public IActionResult DeleteStaff(int id)
        {
            _staffService.DeleteStaff(id);
            return RedirectToAction("StaffManagement");
        }
    }
}
