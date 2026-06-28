using System.Diagnostics;
using Chapeau.Models.Enums;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICommentService _commentService;

        public OrderController(IOrderService orderService, ICommentService commentService)
        {
            _orderService = orderService;
            _commentService = commentService;
        }

        public IActionResult CreateOrder(int tableId, string category = null, string subCategory = null)
        {
            try
            {
                int staffId = HttpContext.Session.GetInt32("StaffId") ?? 0;
                var displayPage = _orderService.GetOrderPage(tableId, staffId, category, subCategory);
                return View("CreateOrder", displayPage);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while trying to load the page.";
                return RedirectToAction("Index", "RestaurantTable");
            }
        }

        [HttpPost]
        public IActionResult AddItem(int menuItemId, int tableId, string category = null, string subCategory = null)
        {
            try
            {
                _orderService.AddItemToTable(menuItemId, tableId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Could not add the item. Try again.";
            }

            return RedirectToAction("CreateOrder", new { tableId = tableId, category = category, subCategory = subCategory });
        }

        [HttpPost]
        public IActionResult DecreaseItem(int orderItemId, int tableId,string category = null, string subCategory = null)
        {
            try
            {
                _orderService.DecreaseItemForTable(orderItemId, tableId);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while updating order. Try again.";
            }

            return RedirectToAction("CreateOrder", new { tableId = tableId, category = category, subCategory = subCategory });
        }

        [HttpPost]
        public IActionResult DeleteItem(int orderItemId, int tableId, string category = null, string subCategory = null)
        {
            try
            {
                _orderService.RemoveItemFromTable(orderItemId, tableId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while removing the item. Try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId, category = category, subCategory = subCategory });
        }

        [HttpPost]
        public IActionResult UpdateItemNote(int orderItemId, int tableId, string note, string category = null, string subCategory = null)
        {
            try
            {
                _orderService.UpdateOrderItemNote(orderItemId, note);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while updating the note. Try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId, category = category, subCategory = subCategory });
        }

        [HttpPost]
        public IActionResult ServeItem(int orderItemId, int tableId, string category = null, string subCategory = null)
        {
            try
            {
                _orderService.ServeOrderItem(orderItemId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something went wrong while serving the item. Please try again.";
            }
            return RedirectToAction("CreateOrder", new { tableId = tableId, category = category, subCategory = subCategory });
        }
    }
}
