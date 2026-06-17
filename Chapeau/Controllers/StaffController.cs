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
            int.TryParse(Request.Cookies["SelectedStaffId"], out int selectedId);
            ViewBag.SelectedStaffId = selectedId;

            return View(_staffService.GetAllStaffWithoutPins());
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

            Staff staff = _staffService.TryLogin(staffId, password);

            if (staff == null)
            {
                ViewBag.Error = "Invalid password.";
                ViewBag.SelectedStaffId = staffId;
                return View(_staffService.GetAllStaffWithoutPins());
            }

            HttpContext.Session.SetString("StaffName", staff.Name);
            HttpContext.Session.SetString("StaffRole", staff.Role.ToString().ToLower());
            HttpContext.Session.SetInt32("StaffId", staff.StaffId);

            Response.Cookies.Delete("SelectedStaffId");

            return staff.Role switch
            {
                Models.Enums.StaffRole.Chef => RedirectToAction("Index", "Kitchen"),
                Models.Enums.StaffRole.Bar  => RedirectToAction("Index", "Bar"),
                _                           => RedirectToAction("Index", "RestaurantTable")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("SelectedStaffId");
            return RedirectToAction("Login");
        }
    }
}
