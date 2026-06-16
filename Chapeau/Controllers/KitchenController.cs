using Chapeau.Models;
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
        private readonly IStaffService _staffService;

        public KitchenController(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IStaffService staffService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _staffService = staffService;
        }

        public IActionResult Index()
        {
            List<Staff> staff = _staffService.GetAllStaff();
            List<Order> allOrders = _orderService.GetAllOrders();
            List<OrderItem> allItems = _orderItemService.GetAllOrderItems();

            // Group food items that are still waiting, keyed by their order id
            Dictionary<int, List<OrderItem>> itemsByOrder = new Dictionary<int, List<OrderItem>>();
            foreach (OrderItem item in allItems)
            {
                if (item.ItemType != ItemType.Food || item.Status != OrderItemStatus.Ordered)
                    continue;

                int orderId = item.Order != null ? item.Order.OrderId : 0;
                if (!itemsByOrder.ContainsKey(orderId))
                    itemsByOrder[orderId] = new List<OrderItem>();

                itemsByOrder[orderId].Add(item);
            }

            List<KitchenOrderCard> cards = new List<KitchenOrderCard>();
            foreach (Order order in allOrders)
            {
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress)
                    continue;
                if (!itemsByOrder.ContainsKey(order.OrderId))
                    continue;

                List<OrderItem> items = itemsByOrder[order.OrderId];

                int orderStaffId = order.Staff != null ? order.Staff.StaffId : 0;
                string staffName = "Unknown";
                foreach (Staff s in staff)
                {
                    if (s.StaffId == orderStaffId)
                    {
                        staffName = s.Name;
                        break;
                    }
                }

                DateTime orderedAt = items[0].CreatedAt;
                List<KitchenItemRow> rows = new List<KitchenItemRow>();
                foreach (OrderItem item in items)
                {
                    if (item.CreatedAt < orderedAt)
                        orderedAt = item.CreatedAt;

                    rows.Add(new KitchenItemRow
                    {
                        Name = item.Name,
                        Qty = item.Qty,
                        CreatedAt = item.CreatedAt,
                        Note = item.Note
                    });
                }

                cards.Add(new KitchenOrderCard
                {
                    OrderId = order.OrderId,
                    TableId = order.Table != null ? order.Table.TableId : 0,
                    StaffName = staffName,
                    OrderedAt = orderedAt,
                    Items = rows
                });
            }

            // Oldest (most urgent) first
            cards.Sort((a, b) => b.MinutesAgo.CompareTo(a.MinutesAgo));

            return View(cards);
        }

        [HttpPost]
        public IActionResult MarkReady(int orderId)
        {
            _orderItemService.MarkOrderItemsReady(orderId, ItemType.Food);
            return RedirectToAction("Index");
        }
    }
}
