using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Index(int tableId)
        {
            BillViewModel bill = _paymentService.GetBill(tableId);

            if (bill == null)
                return RedirectToAction("Index", "RestaurantTable");

            return View(bill);
        }

        [HttpPost]
        public IActionResult Process(int tableId, int orderId, decimal tip, decimal tipOther, string paymentMethod, string splitMethod, int splitWays = 1)
        {
            _paymentService.ProcessPayment(tableId, orderId, tip, tipOther, paymentMethod, splitWays);
            return RedirectToAction("Index", "RestaurantTable");
        }
    }
}
