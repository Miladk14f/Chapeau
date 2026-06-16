using Chapeau.Models.Enums;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class BarController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;

        public BarController(IOrderService orderService, IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public IActionResult Index()
        {
            return View(_orderService.GetPreparationCards(ItemType.Drink, 6, 10));
        }

        [HttpPost]
        public IActionResult MarkReady(int orderId)
        {
            _orderItemService.MarkOrderItemsReady(orderId, ItemType.Drink);
            return RedirectToAction("Index");
        }
    }
}
