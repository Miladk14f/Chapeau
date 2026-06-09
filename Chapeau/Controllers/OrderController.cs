using Chapeau.Models;
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

        public IActionResult CreateOrder(int tableId)
        {
            RestaurantTable tableOrder = _tableService.GetTableById(tableId);
            List<OrderItem> orderItems = new List<OrderItem>();
            List<MenuItem> menuItems = _menuItemService.GetAllMenuItems();
            Staff staff = _staffService.GetStaffById(2);

            OrderViewModel orderViewModel = new OrderViewModel(menuItems, orderItems, tableOrder, staff);

            return View("CreateOrder", orderViewModel);
        }
    }
}