using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class RestaurantTableController : Controller
    {
        private readonly IRestaurantTableService _tableService;
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;

        public RestaurantTableController(IRestaurantTableService tableService, IOrderService orderService, IOrderItemService orderItemService)
        {
            _tableService = tableService;
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public IActionResult Index()
        {
            List<RestaurantTable> tables = _tableService.GetAllTables();
            List<Order> orders = _orderService.GetAllOrders();
            List<OrderItem> items = _orderItemService.GetAllOrderItems();

            Dictionary<int, int> tableIdByOrder = new Dictionary<int, int>();
            foreach (Order order in orders)
            {
                int tableId = order.Table != null ? order.Table.TableId : 0;
                tableIdByOrder[order.OrderId] = tableId;
            }

            Dictionary<int, TableCard> cardByTable = new Dictionary<int, TableCard>();
            foreach (RestaurantTable table in tables)
                cardByTable[table.TableId] = new TableCard { Table = table };

            foreach (OrderItem item in items)
            {
                if (item.Status != OrderItemStatus.Ready)
                    continue;

                int orderId = item.Order != null ? item.Order.OrderId : 0;
                if (!tableIdByOrder.ContainsKey(orderId))
                    continue;

                int tableId = tableIdByOrder[orderId];
                if (!cardByTable.ContainsKey(tableId))
                    continue;

                ReadyItemRow row = new ReadyItemRow { Name = item.Name, Qty = item.Qty };
                if (item.ItemType == ItemType.Drink)
                    cardByTable[tableId].ReadyDrink.Add(row);
                else
                    cardByTable[tableId].ReadyFood.Add(row);
            }

            TableOverviewViewModel vm = new TableOverviewViewModel();
            foreach (RestaurantTable table in tables)
                vm.Cards.Add(cardByTable[table.TableId]);

            return View(vm);
        }

        public IActionResult Details(int id)
        {
            RestaurantTable table = _tableService.GetTableById(id);

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
            List<RestaurantTable> tables = _tableService.GetAllTables()
                .Where(t => t.Status != TableStatus.Occupied)
                .ToList();
            return View(tables);
        }

        [HttpPost]
        public IActionResult Reserve(int tableId, string reservationName, int guests, DateTime reservationAt)
        {
            _tableService.ReserveTable(tableId, reservationName, guests, reservationAt);
            return RedirectToAction("Index");
        }
    }
}
