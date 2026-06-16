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
            return View(_tableService.GetTableOverview());
        }

        public IActionResult Details(int id)
        {
            var table = _tableService.GetTableById(id);

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

        [HttpGet]
        public IActionResult Reserve()
        {
            return View(_tableService.GetAvailableTables());
        }

        [HttpPost]
        public IActionResult Reserve(int tableId, string reservationName, int guests, DateTime reservationAt)
        {
            _tableService.ReserveTable(tableId, reservationName, guests, reservationAt);
            return RedirectToAction("Index");
        }
    }
}
