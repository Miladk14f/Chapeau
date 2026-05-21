using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Controllers
{
    public class BarController : Controller
    {
        private readonly StockItemService _stockService;

        public BarController(StockItemService stockService)
        {
            _stockService = stockService;
        }

        public IActionResult Index()
        {
            List<StockItem> inventory = _stockService.GetInventory();
            
            return View(inventory);
        }
    }
}