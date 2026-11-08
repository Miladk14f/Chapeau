// Author: Student3
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    /// <summary>Handles login, logout and access-denied for cookie authentication.</summary>
    public class AccountController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AccountController(IEmployeeService employeeService)
            => _employeeService = employeeService;

        // GET /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var employee = await _employeeService.ValidateLoginAsync(model.Email, model.Password);

                if (employee == null)
                {
                    // Generic error — do not reveal whether email or password was wrong
                    ModelState.AddModelError(string.Empty, "An incorrect password has been entered.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, employee.Id),
                    new(ClaimTypes.Name,           employee.Name),
                    new(ClaimTypes.Email,          employee.Email),
                    new(ClaimTypes.Role,           employee.Role)
                };

                var identity  = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                var authProps = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc   = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync("Cookies", principal, authProps);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
                return View(model);
            }
        }

        // POST /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        // GET /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
