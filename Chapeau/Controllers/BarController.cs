using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class BarController : Controller
    {
        private readonly IStockItemService _stockService;

        public BarController(IStockItemService stockService)
        {
            _stockService = stockService;
        }

        public IActionResult Index()
        {
            List<StockItem> inventory = _stockService.GetAll();
            return View(inventory);
        }
    }
}
