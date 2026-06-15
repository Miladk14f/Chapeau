using Chapeau.Models;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.ViewModels;
using Chapeau.Extentions;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;
        private readonly IStaffService _staffService;
        private readonly IRestaurantTableService _tableService;

        public OrderController(IOrderService orderService, IMenuItemService menuItemService, IStaffService staffService, IRestaurantTableService tableService)
        {
            _menuItemService = menuItemService;
            _orderService = orderService;
            _staffService = staffService;
            _tableService = tableService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateOrder(int tableId) // int staffId from session (waiting on teammate)
        {
            RestaurantTable tableOrder = _tableService.GetTableById(tableId);


            List<OrderItem> orderItems = HttpContext.Session.GetObject<List<OrderItem>>("OrderItemsList");
            if (orderItems == null)
            {
                orderItems = new List<OrderItem>();
            }
           
            List<MenuItem> menuItems = _menuItemService.GetAllMenuItems();

            Staff staff = _staffService.GetStaffById(2); // staffId

            OrderViewModel orderViewModel = new OrderViewModel(menuItems, orderItems, tableOrder, staff);


            return View("CreateOrder", orderViewModel);
        }


        [HttpPost]
        public IActionResult AddItem(int menueItemId, int tableId)
        {

            List<OrderItem> orderItems = HttpContext.Session.GetObject<List<OrderItem>>("OrderItemsList");
            if (orderItems == null)
            {
                orderItems = new List<OrderItem>();
            }

            // check if item is in list?
            foreach (OrderItem orderItem in orderItems)
            {
                if (orderItem.MenuItem.MenuItemId != menueItemId)
                {
                    orderItem.Qty++;
                }
                else
                {
                    // if not found in list -> create that item
                    MenuItem menuItemForOrder = _menuItemService.GetMenuItemById(menueItemId);

                    // creating new order item 
                    OrderItem newOrderItem = new OrderItem
                    {
                        Name = menuItemForOrder.Name,
                        Price = menuItemForOrder.Price,
                        Vat = menuItemForOrder.Vat,
                        ItemType = menuItemForOrder.Category,
                        Qty = 1,
                        CreatedAt = DateTime.Now,
                        MenuItem = menuItemForOrder
                    };

                    orderItems.Add(newOrderItem);
                }
            }
            HttpContext.Session.SetObject("OrderItemsList", orderItems);

            return RedirectToAction("CreateOrder");
        }




        [HttpPost]
        public IActionResult RemoveItem(int orderItemId, int tableId)
        {
            List<OrderItem> orderItems = HttpContext.Session.GetObject<List<OrderItem>>("OrderItemsList");

            // if null??? if there is no list to get = show something? - dont think there is a way to get here without a list.. 

            // how do you press "-" on an item that doesnt exist in the list?

            foreach(OrderItem orderItem in orderItems)
            {
                if (orderItem.MenuItem.MenuItemId == orderItemId)
                {
                    if (orderItem.Qty > 1)
                    {
                        orderItem.Qty--;
                    }
                    else
                    {
                        // if qty becomes 0 after - than remove from list
                        orderItems.Remove(orderItem);
                    }
                }
            }





            HttpContext.Session.SetObject("OrderItemsList", orderItems);

            return RedirectToAction("CreateOrder");
        }
    }
}