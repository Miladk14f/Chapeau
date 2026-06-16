using Chapeau.Models.Enums;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class KitchenController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;

        public KitchenController(IOrderService orderService, IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public IActionResult Index()
        {
            return View(_orderService.GetPreparationCards(ItemType.Food, 12, 20));
        }

        [HttpPost]
        public IActionResult StartPreparing(int orderId)
        {
            _orderItemService.StartPreparingItems(orderId, ItemType.Food);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkReady(int orderId)
        {
            _orderItemService.MarkOrderItemsReady(orderId, ItemType.Food);
            return RedirectToAction("Index");
        }
    }
}
