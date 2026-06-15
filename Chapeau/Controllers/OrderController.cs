using Chapeau.Models;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.ViewModels;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;
        private readonly IStaffService _staffService;
        private readonly IRestaurantTableService _tableService;
        private readonly IOrderItemService _orderItemService;

        public OrderController(IOrderService orderService, IMenuItemService menuItemService, IStaffService staffService, IRestaurantTableService tableService, IOrderItemService orderItemService)
        {
            _menuItemService = menuItemService;
            _orderService = orderService;
            _staffService = staffService;
            _tableService = tableService;
            _orderItemService = orderItemService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateOrder(int tableId)
        {
            RestaurantTable tableOrder = _tableService.GetTableById(tableId);
            List<MenuItem> menuItems = _menuItemService.GetAllMenuItems();
            Staff staff = _staffService.GetStaffById(2);

            // get existing order - if not found, create a new onw
            Order currentOrder = _orderService.GetActiveOrderByTableId(tableId);
            if (currentOrder == null)
            {
                int newOrderId = _orderService.AddOrder(new Order
                {
                    Table = tableOrder,
                    Staff = staff,
                    Status = Models.Enums.OrderStatus.InProgress,
                    CreatedAt = DateTime.Now
                });
                currentOrder = _orderService.GetOrderById(newOrderId);
            }

            List<OrderItem> orderItems = _orderItemService.GetOrderItemsByTableId(tableId);

            OrderViewModel orderViewModel = new OrderViewModel(menuItems, orderItems, tableOrder, staff);
            return View("CreateOrder", orderViewModel);
        }


        [HttpPost]
        public IActionResult AddItem(int menuItemId, int tableId)
        {
            Order currentOrder = _orderService.GetActiveOrderByTableId(tableId);

            // Load current items to check if this menu item is already in the order
            List<OrderItem> orderItems = _orderItemService.GetOrderItemsByTableId(tableId);
            OrderItem existingItem = orderItems.FirstOrDefault(item => item.MenuItem.MenuItemId == menuItemId);

            if (existingItem != null)
            {
                existingItem.Qty++;
                _orderItemService.UpdateOrderItem(existingItem);
            }
            else
            {
                // if item not in list - create it using the menue item
                MenuItem menuItemForOrder = _menuItemService.GetMenuItemById(menuItemId);

                OrderItem newItem = new OrderItem
                {
                    Order = currentOrder,
                    MenuItem = menuItemForOrder,
                    Name = menuItemForOrder.Name,
                    Price = menuItemForOrder.Price,
                    Vat = menuItemForOrder.Vat,
                    ItemType = menuItemForOrder.Category,
                    Qty = 1,
                    CreatedAt = DateTime.Now
                };

                _orderItemService.AddOrderItem(newItem);
            }

            _tableService.UpdateTableStatus(tableId, Models.Enums.TableStatus.Occupied);

            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }


        [HttpPost]
        public IActionResult DecreaseItem(int orderItemId, int tableId)
        {
            OrderItem itemToDecrease = _orderItemService.GetOrderItemById(orderItemId);

            if (itemToDecrease != null)
            {
                if (itemToDecrease.Qty > 1)
                {
                    itemToDecrease.Qty--;
                    _orderItemService.UpdateOrderItem(itemToDecrease);
                }
                else
                {
                    // Qty would hit 0 — remove the row entirely
                    _orderItemService.DeleteOrderItem(orderItemId);
                }
            }

            UpdateTableStatusByItems(tableId);

            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }


        [HttpPost]
        public IActionResult DeleteItem(int orderItemId, int tableId)
        {
            _orderItemService.DeleteOrderItem(orderItemId);

            UpdateTableStatusByItems(tableId);

            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        private void UpdateTableStatusByItems(int tableId)
        {
            List<OrderItem> orderItems = _orderItemService.GetOrderItemsByTableId(tableId);
            TableStatus status = orderItems.Any() ? TableStatus.Occupied : TableStatus.Free;
            _tableService.UpdateTableStatus(tableId, status);
        }

    }
}