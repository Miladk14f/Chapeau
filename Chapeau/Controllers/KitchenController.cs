using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.ViewModels;
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
            var vm = new PreparationPageViewModel
            {
                Cards = _orderService.GetPreparationCards(ItemType.Food, 12, 20),
                PageClass = "kitchen-page",
                HeaderClass = "kitchen-header",
                IconClass = "kitchen-icon",
                IconEmoji = "🍳",
                SubClass = "kitchen-sub",
                Title = "Kitchen",
                CountLabel = "orders",
                PrepLabel = "👨‍🍳 In preparation",
                EmptyText = "No pending food orders.",
                ShowTypeBadge = false,
                ShowZeroUrgent = true,
                LegendNormal = "< 12 min",
                LegendWarning = "12–19 min",
                LegendUrgent = "≥ 20 min (urgent)"
            };
            return View(vm);
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
