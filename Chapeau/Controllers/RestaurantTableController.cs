using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class RestaurantTableController : Controller
    {
        private readonly IRestaurantTableService _tableService;

        public RestaurantTableController(IRestaurantTableService tableService)
        {
            _tableService = tableService;
        }

        public IActionResult Index()
        {
            List<RestaurantTable> tables = _tableService.GetAllTables();
            return View(tables);
        }

        public IActionResult Details(int id)
        {
            RestaurantTable table = _tableService.GetTableById(id);

            if (table == null)
                return NotFound();

            return View(table);
        }

        [HttpPost]
        public IActionResult SeatGuests(int tableId, int guests)
        {
            int staffId = HttpContext.Session.GetInt32("StaffId") ?? 0;
            _tableService.SeatGuestsAtTable(tableId, guests, staffId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult FreeTable(int tableId)
        {
            _tableService.ClearTable(tableId);
            return RedirectToAction("Index");
        }
    }
}
