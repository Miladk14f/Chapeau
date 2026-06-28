using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class KitchenController : Controller
    {
        private readonly IOrderService _orderService;

        public KitchenController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private bool CanAccessKitchen()
        {
            string role = HttpContext.Session.GetString("StaffRole") ?? "";
            return role == "chef" || role == "manager" || role == "waiter";
        }

        public IActionResult Index()
        {
            if (!CanAccessKitchen())
                return RedirectToAction("Login", "Staff");

            var vm = new PreparationPageViewModel
            {
                Cards = _orderService.GetPreparationCards(ItemTypeGroups.Food, 12, 20),
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
            _orderService.StartPreparingItems(orderId, ItemTypeGroups.Food);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkReady(int orderId)
        {
            _orderService.MarkOrderItemsReady(orderId, ItemTypeGroups.Food);
            return RedirectToAction("Index");
        }

        public IActionResult History()
        {
            if (!CanAccessKitchen())
                return RedirectToAction("Login", "Staff");

            return View("~/Views/Shared/OrderHistory.cshtml", new OrderHistoryViewModel
            {
                Cards = _orderService.GetOrderHistory(ItemTypeGroups.Food),
                Title = "Kitchen History",
                PageClass = "kitchen-page",
                BackAction = "Kitchen"
            });
        }

        [HttpPost]
        public IActionResult MarkItemReady(int orderItemId)
        {
            _orderService.UpdateOrderItemStatus(orderItemId, Models.Enums.OrderItemStatus.Ready);
            return RedirectToAction("Index");
        }
    }
}
