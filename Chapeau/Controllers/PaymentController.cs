using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Services;
using Chapeau.Services.BillService;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IBillService _billService;
        private readonly IPaymentService _paymentService;
        private readonly IRestaurantTableService _tableService;
        private readonly IStaffService _staffService;

        public PaymentController(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IBillService billService,
            IPaymentService paymentService,
            IRestaurantTableService tableService,
            IStaffService staffService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _billService = billService;
            _paymentService = paymentService;
            _tableService = tableService;
            _staffService = staffService;
        }

        [HttpGet]
        public IActionResult Index(int tableId)
        {
            var table = _tableService.GetTableById(tableId);
            if (table == null) return NotFound();

            var orders = _orderService.GetOrdersByTableId(tableId)
                .Where(o => o.Status != OrderStatus.Paid)
                .ToList();

            if (!orders.Any())
                return RedirectToAction("Index", "RestaurantTable");

            var activeOrder = orders.OrderByDescending(o => o.CreatedAt).First();
            var items = _orderItemService.GetOrderItemsByOrderId(activeOrder.OrderId);
            var staff = _staffService.GetAllStaff();

            string waiterName = "";
            if (table.Waiter != null)
            {
                var waiter = staff.FirstOrDefault(s => s.StaffId == table.Waiter.StaffId);
                waiterName = waiter?.Name ?? "";
            }

            var items9 = items.Where(i => i.Vat == 9).ToList();
            var items21 = items.Where(i => i.Vat == 21).ToList();

            decimal sum9 = items9.Sum(i => i.Price * i.Qty);
            decimal sum21 = items21.Sum(i => i.Price * i.Qty);
            decimal excl9 = sum9 > 0 ? Math.Round(sum9 / 1.09m, 2) : 0;
            decimal excl21 = sum21 > 0 ? Math.Round(sum21 / 1.21m, 2) : 0;

            BillItemRow ToRow(OrderItem item)
            {
                var member = staff.FirstOrDefault(s => s.StaffId == (activeOrder.Staff?.StaffId ?? 0));
                return new BillItemRow
                {
                    Name = item.Name,
                    Qty = item.Qty,
                    UnitPrice = item.Price,
                    StaffName = member?.Name ?? "",
                    Vat = item.Vat
                };
            }

            var vm = new BillViewModel
            {
                TableId = tableId,
                OrderId = activeOrder.OrderId,
                Guests = table.Guests ?? 0,
                WaiterName = waiterName,
                GeneratedAt = DateTime.Now,
                Items9 = items9.Select(ToRow).ToList(),
                Items21 = items21.Select(ToRow).ToList(),
                Excl9 = excl9,
                Vat9Amount = Math.Round(sum9 - excl9, 2),
                Excl21 = excl21,
                Vat21Amount = Math.Round(sum21 - excl21, 2)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Process(int tableId, int orderId, decimal tip, decimal tipOther, string paymentMethod, string splitMethod, int splitWays = 1)
        {
            var items = _orderItemService.GetOrderItemsByOrderId(orderId);
            if (tipOther > 0) tip = tipOther;
            decimal total = items.Sum(i => i.Price * i.Qty) + tip;

            SplitMethod split = splitWays > 1 ? SplitMethod.Equal : SplitMethod.None;

            var bill = new Bill
            {
                Tip = tip,
                SplitedMethod = split,
                Amount = total,
                Order = new Order { OrderId = orderId }
            };

            int billId = _billService.AddBill(bill);

            PaymentMethod method = paymentMethod?.ToLower() switch
            {
                "cash" => PaymentMethod.Cash,
                _ => PaymentMethod.Pin
            };

            int ways = Math.Max(1, splitWays);
            decimal perPerson = Math.Round(total / ways, 2);

            for (int i = 0; i < ways; i++)
            {
                decimal amount = i == ways - 1 ? total - perPerson * (ways - 1) : perPerson;
                _paymentService.AddPayment(new Payment
                {
                    PaymentMethod = method,
                    Amount = amount,
                    Status = BillStatus.Paid,
                    PaidAt = DateTime.Now,
                    Bill = new Bill { BillId = billId }
                });
            }

            _billService.UpdateBill(new Bill
            {
                BillId = billId,
                Tip = tip,
                SplitedMethod = split,
                Amount = total,
                Status = BillStatus.Paid,
                Order = new Order { OrderId = orderId }
            });

            _orderService.UpdateOrderStatus(orderId, OrderStatus.Paid);
            _tableService.ClearTable(tableId);

            return RedirectToAction("Index", "RestaurantTable");
        }
    }
}
