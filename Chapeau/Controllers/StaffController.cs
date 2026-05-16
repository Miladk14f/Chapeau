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
            return View();
        }

        [HttpPost]
        public IActionResult Login(string staffName, string password)
        {
            Staff staff = _staffService.Login(staffName, password);

            if (staff == null)
            {
                ViewBag.Error = "Invalid name or password.";
                return View();
            }

            HttpContext.Session.SetString("StaffName", staff.Name);
            HttpContext.Session.SetString("StaffRole", staff.Role);
            HttpContext.Session.SetInt32("StaffId", staff.Id);

            return staff.Role switch
            {
                "manager" => RedirectToAction("Index", "Home"),
                "chef"    => RedirectToAction("Index", "Home"),
                "bar"     => RedirectToAction("Index", "Home"),
                "waiter"  => RedirectToAction("Index", "Home"),
                _         => RedirectToAction("Login")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
