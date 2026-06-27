using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly ICommentService _commentService;

        public OrderController(IOrderService orderService, IOrderItemService orderItemService, ICommentService commentService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _commentService = commentService;
        }

        public IActionResult CreateOrder(int tableId)
        {
            int staffId = HttpContext.Session.GetInt32("StaffId") ?? 0;
            return View("CreateOrder", _orderService.GetOrderPage(tableId, staffId));
        }

        [HttpPost]
        public IActionResult AddItem(int menuItemId, int tableId)
        {
            _orderItemService.AddItemToTable(menuItemId, tableId);
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult DecreaseItem(int orderItemId, int tableId)
        {
            _orderItemService.DecreaseItemForTable(orderItemId, tableId);
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult DeleteItem(int orderItemId, int tableId)
        {
            _orderItemService.RemoveItemFromTable(orderItemId, tableId);
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult UpdateItemNote(int orderItemId, int tableId, string note)
        {
            _orderItemService.UpdateOrderItemNote(orderItemId, note);
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult ServeItem(int orderItemId, int tableId)
        {
            _orderItemService.ServeOrderItem(orderItemId);
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult AddComment(int tableId, string type, string text)
        {
            _commentService.AddCommentForTable(tableId, type, text);
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }
    }
}
