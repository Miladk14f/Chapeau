using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            List<Staff> staff = _staffService.GetAllStaff();

            int.TryParse(Request.Cookies["SelectedStaffId"], out int selectedId);
            ViewBag.SelectedStaffId = selectedId;

            return View(staff);
        }

        [HttpPost]
        public IActionResult Select(int staffId)
        {
            Response.Cookies.Append("SelectedStaffId", staffId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            int.TryParse(Request.Cookies["SelectedStaffId"], out int staffId);

            Staff staff = _staffService.GetStaffById(staffId);

            if (staff == null || !_staffService.VerifyStaffPin(staff, password))
            {
                ViewBag.Error = "Invalid password.";
                ViewBag.SelectedStaffId = staffId;
                return View(_staffService.GetAllStaff());
            }

            HttpContext.Session.SetString("StaffName", staff.Name);
            HttpContext.Session.SetString("StaffRole", staff.Role.ToString().ToLower());
            HttpContext.Session.SetInt32("StaffId", staff.StaffId);

            Response.Cookies.Delete("SelectedStaffId");

            return RedirectToAction("Index", "RestaurantTable");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("SelectedStaffId");
            return RedirectToAction("Login");
        }
    }
}
