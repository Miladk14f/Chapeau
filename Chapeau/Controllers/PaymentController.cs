using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ICommentService _commentService;
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

        public PaymentController(IPaymentService paymentService, ICommentService commentService)
        {
            _paymentService = paymentService;
            _commentService = commentService;
        }

        [HttpGet]
        public IActionResult Index(int tableId)
        {
            BillViewModel bill = _paymentService.GetBill(tableId);
            if (bill == null)
                return RedirectToAction("Index", "RestaurantTable");

            string splitJson = HttpContext.Session.GetString($"split_{tableId}");
            if (splitJson != null)
            {
                SplitData split = JsonSerializer.Deserialize<SplitData>(splitJson, JsonOpts);
                bill.SplitPersons = split.Persons;
                bill.SplitBillId = split.BillId;
            }

            return View(bill);
        }

        // Single payment (no split)
        [HttpPost]
        public IActionResult Process(int tableId, int orderId, string personsJson)
        {
            List<PersonPaymentInput> persons = JsonSerializer.Deserialize<List<PersonPaymentInput>>(personsJson, JsonOpts)
                                              ?? new List<PersonPaymentInput>();
            _paymentService.ProcessPayment(tableId, orderId, persons);

            foreach (PersonPaymentInput person in persons)
            {
                if (!string.IsNullOrWhiteSpace(person.FeedbackText))
                    _commentService.AddCommentForTable(tableId, person.FeedbackType ?? "Comment", person.FeedbackText);
            }

            return RedirectToAction("Index", "RestaurantTable");
        }

        // Configure split — creates Bill, stores persons in session
        [HttpPost]
        public IActionResult SetupSplit(int tableId, int orderId, string personsJson)
        {
            List<PersonPaymentInput> persons = JsonSerializer.Deserialize<List<PersonPaymentInput>>(personsJson, JsonOpts)
                                              ?? new List<PersonPaymentInput>();

            SplitData split = _paymentService.StartSplitBill(orderId, persons);
            HttpContext.Session.SetString($"split_{tableId}", JsonSerializer.Serialize(split));
            return RedirectToAction("Index", new { tableId });
        }

        // Pay one person's portion
        [HttpPost]
        public IActionResult PayPerson(int tableId, int orderId, int personIndex)
        {
            string key = $"split_{tableId}";
            string splitJson = HttpContext.Session.GetString(key);
            if (splitJson == null)
                return RedirectToAction("Index", new { tableId });

            SplitData split = JsonSerializer.Deserialize<SplitData>(splitJson, JsonOpts);
            SplitPersonState person = null;
            foreach (SplitPersonState p in split.Persons)
            {
                if (p.Index == personIndex) { person = p; break; }
            }

            if (person == null || person.Paid)
                return RedirectToAction("Index", new { tableId });

            _paymentService.AddSplitPersonPayment(split.BillId, person.Total, person.PaymentMethod);

            if (!string.IsNullOrWhiteSpace(person.FeedbackText))
                _commentService.AddCommentForTable(tableId, person.FeedbackType ?? "Comment", person.FeedbackText);

            person.Paid = true;
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(split));

            if (split.AllPaid)
            {
                _paymentService.CloseTable(tableId, orderId);
                HttpContext.Session.Remove(key);
                return RedirectToAction("Index", "RestaurantTable");
            }

            return RedirectToAction("Index", new { tableId });
        }

        // Cancel split — remove session state
        [HttpPost]
        public IActionResult CancelSplit(int tableId)
        {
            HttpContext.Session.Remove($"split_{tableId}");
            return RedirectToAction("Index", new { tableId });
        }
    }
}
