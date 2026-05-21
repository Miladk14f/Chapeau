using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index(string tab = "revenue", string stockFilter = "all")
        {
            var vm = new ManagerDashboardViewModel
            {
                ActiveTab = tab,
                StockFilter = stockFilter,

                RevenuePaid = 0,
                RevenueOpen = 0,
                CoversToday = 0,
                CoversSeated = 0,
                TipsReceived = 0,
                TablesCheckedOut = 0,
                StockOutCount = 0,
                StockLowCount = 0,

                CategoryRevenues = new(),
                StaffPerformances = new(),
                StockItems = new(),
                FeedbackItems = new(),
                OpenTables = new(),
            };

            return View(vm);
        }
    }
}
