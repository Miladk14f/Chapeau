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
        private readonly IStaffService _staffService;

        public BarController(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IStaffService staffService)
        {
            _orderService     = orderService;
            _orderItemService = orderItemService;
            _staffService     = staffService;
        }


        public IActionResult Index()
        {
            var staff  = _staffService.GetAllStaff();
            var orders = _orderService.GetAllOrders()
                .Where(o => o.Status == EOrderStatus.Pending || o.Status == EOrderStatus.InProgress)
                .ToList();

            var allItems = _orderItemService.GetAllOrderItems()
                .Where(i => i.ItemType == EItemType.Drink)
                .GroupBy(i => i.Order?.OrderId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            var cards = orders
                .Where(o => allItems.ContainsKey(o.OrderId))
                .Select(o =>
                {
                    var items     = allItems[o.OrderId];
                    var member    = staff.FirstOrDefault(s => s.StaffId == (o.Staff?.StaffId ?? 0));
                    var orderedAt = items.Min(i => i.CreatedAt);

                    return new BarOrderCard
                    {
                        OrderId   = o.OrderId,
                        TableId   = o.Table?.TableId ?? 0,
                        StaffName = member?.Name ?? "Unknown",
                        OrderedAt = orderedAt,
                        Items     = items.Select(i => new KitchenItemRow
                        {
                            Name      = i.Name,
                            Qty       = i.Qty,
                            CreatedAt = i.CreatedAt
                        }).ToList()
                    };
                })
                .OrderByDescending(c => c.MinutesAgo)
                .ToList();

            return View(cards);
        }

        [HttpPost]
        public IActionResult MarkReady(int orderId)
        {
            _orderService.UpdateOrderStatus(orderId, EOrderStatus.Prepared);
            return RedirectToAction("Index");
        }
    }
}

