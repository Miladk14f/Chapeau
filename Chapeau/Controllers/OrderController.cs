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
            try
            {
                int staffId = HttpContext.Session.GetInt32("StaffId") ?? 0;
                var displayPage = _orderService.GetOrderPage(tableId, staffId);
                return View("CreateOrder", displayPage);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while trying load the page.";
                return RedirectToAction("Index", "RestaurantTable");
            }
        }

        [HttpPost]
        public IActionResult AddItem(int menuItemId, int tableId)
        {
            try
            {
                _orderItemService.AddItemToTable(menuItemId, tableId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "COuld not add item. Try again.";
            }

            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult DecreaseItem(int orderItemId, int tableId)
        {
            try
            {
                _orderItemService.DecreaseItemForTable(orderItemId, tableId);

            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while updating order. Try again.";
            }

            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult DeleteItem(int orderItemId, int tableId)
        {
            try
            {
                _orderItemService.RemoveItemFromTable(orderItemId, tableId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while removing the item. Try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult UpdateItemNote(int orderItemId, int tableId, string note)
        {
            try
            {
                _orderItemService.UpdateOrderItemNote(orderItemId, note);

            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while updating the note. Try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult ServeItem(int orderItemId, int tableId)
        {
            try
            {
                _orderItemService.ServeOrderItem(orderItemId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while serving the item. Please try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }

        [HttpPost]
        public IActionResult AddComment(int tableId, string type, string text)
        {
            try
            {
                _commentService.AddCommentForTable(tableId, type, text);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while saving the comment. Please try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId });
        }
    }
}
