using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.ViewModels;
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

        private bool CanAccessBar()
        {
            string role = HttpContext.Session.GetString("StaffRole") ?? "";
            return role == "bar" || role == "manager" || role == "waiter";
        }

        public IActionResult Index()
        {
            if (!CanAccessBar())
                return RedirectToAction("Login", "Staff");

            var vm = new PreparationPageViewModel
            {
                Cards = _orderService.GetPreparationCards(ItemTypeGroups.Drinks, 6, 10),
                PageClass = "bar-page",
                HeaderClass = "bar-header",
                IconClass = "bar-icon",
                IconEmoji = "🍷",
                SubClass = "bar-sub",
                Title = "Bar",
                CountLabel = "pending",
                PrepLabel = "🍸 In preparation",
                EmptyText = "No pending drink orders.",
                ShowTypeBadge = true,
                ShowZeroUrgent = false,
                LegendNormal = "< 6 min",
                LegendWarning = "6–9 min",
                LegendUrgent = "≥ 10 min (urgent)"
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult StartPreparing(int orderId)
        {
            _orderItemService.StartPreparingItems(orderId, ItemTypeGroups.Drinks);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkReady(int orderId)
        {
            _orderItemService.MarkOrderItemsReady(orderId, ItemTypeGroups.Drinks);
            return RedirectToAction("Index");
        }

        public IActionResult History()
        {
            if (!CanAccessBar())
                return RedirectToAction("Login", "Staff");

            return View("~/Views/Shared/OrderHistory.cshtml", new OrderHistoryViewModel
            {
                Cards = _orderService.GetOrderHistory(ItemTypeGroups.Drinks),
                Title = "Bar History",
                PageClass = "bar-page",
                BackAction = "Bar"
            });
        }

        [HttpPost]
        public IActionResult MarkItemReady(int orderItemId)
        {
            _orderItemService.UpdateOrderItemStatus(orderItemId, Models.Enums.OrderItemStatus.Ready);
            return RedirectToAction("Index");
        }
    }
}
