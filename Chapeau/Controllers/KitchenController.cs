using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class KitchenController : Controller
    {
        private readonly ICommentService _commentService;

        public KitchenController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public IActionResult Index(int orderId = 0)
        {
            List<Comment> comments = orderId > 0
                ? _commentService.GetByOrderId(orderId)
                : new List<Comment>();

            return View(comments);
        }
    }
}
