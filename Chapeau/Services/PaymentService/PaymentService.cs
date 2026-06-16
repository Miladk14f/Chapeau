using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.Repositories.BillRepository;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBillRepository _billRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IRestaurantTableRepository _tableRepository;
        private readonly IStaffRepository _staffRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBillRepository billRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IRestaurantTableRepository tableRepository,
            IStaffRepository staffRepository)
        {
            _paymentRepository = paymentRepository;
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _tableRepository = tableRepository;
            _staffRepository = staffRepository;
        }

        public BillViewModel GetBill(int tableId)
        {
            RestaurantTable table = _tableRepository.GetTableById(tableId);
            if (table == null)
                return null;

            List<Order> orders = _orderRepository.GetOrdersByTableId(tableId);
            Order activeOrder = null;
            foreach (Order order in orders)
            {
                if (order.Status == OrderStatus.Paid)
                    continue;
                if (activeOrder == null || order.CreatedAt > activeOrder.CreatedAt)
                    activeOrder = order;
            }

            if (activeOrder == null)
                return null;

            List<OrderItem> items = _orderItemRepository.GetOrderItemsByOrderId(activeOrder.OrderId);
            List<Staff> staff = _staffRepository.GetAllStaff();

            string waiterName = "";
            if (table.Waiter != null)
            {
                foreach (Staff s in staff)
                {
                    if (s.StaffId == table.Waiter.StaffId)
                    {
                        waiterName = s.Name;
                        break;
                    }
                }
            }

            int orderStaffId = activeOrder.Staff != null ? activeOrder.Staff.StaffId : 0;
            string orderStaffName = "";
            foreach (Staff s in staff)
            {
                if (s.StaffId == orderStaffId)
                {
                    orderStaffName = s.Name;
                    break;
                }
            }

            List<BillItemRow> items9 = new List<BillItemRow>();
            List<BillItemRow> items21 = new List<BillItemRow>();
            decimal sum9 = 0;
            decimal sum21 = 0;

            foreach (OrderItem item in items)
            {
                BillItemRow row = new BillItemRow
                {
                    Name = item.Name,
                    Qty = item.Qty,
                    UnitPrice = item.Price,
                    StaffName = orderStaffName,
                    Vat = item.Vat
                };

                decimal lineTotal = item.Price * item.Qty;
                if (item.Vat == 9)
                {
                    items9.Add(row);
                    sum9 += lineTotal;
                }
                else if (item.Vat == 21)
                {
                    items21.Add(row);
                    sum21 += lineTotal;
                }
            }

            decimal excl9 = sum9 > 0 ? Math.Round(sum9 / 1.09m, 2) : 0;
            decimal excl21 = sum21 > 0 ? Math.Round(sum21 / 1.21m, 2) : 0;

            return new BillViewModel
            {
                TableId = tableId,
                OrderId = activeOrder.OrderId,
                Guests = table.Guests ?? 0,
                WaiterName = waiterName,
                GeneratedAt = DateTime.Now,
                Items9 = items9,
                Items21 = items21,
                Excl9 = excl9,
                Vat9Amount = Math.Round(sum9 - excl9, 2),
                Excl21 = excl21,
                Vat21Amount = Math.Round(sum21 - excl21, 2)
            };
        }

        public void ProcessPayment(int tableId, int orderId, decimal tip, decimal tipOther, string paymentMethod, int splitWays)
        {
            List<OrderItem> items = _orderItemRepository.GetOrderItemsByOrderId(orderId);
            if (tipOther > 0)
                tip = tipOther;

            decimal itemsTotal = 0;
            foreach (OrderItem item in items)
                itemsTotal += item.Price * item.Qty;

            decimal total = itemsTotal + tip;

            SplitMethod split = splitWays > 1 ? SplitMethod.Equal : SplitMethod.None;

            Bill bill = new Bill
            {
                Tip = tip,
                SplitedMethod = split,
                Amount = total,
                Status = BillStatus.Unpaid,
                Order = new Order { OrderId = orderId }
            };
            int billId = _billRepository.AddBill(bill);

            PaymentMethod method = paymentMethod != null && paymentMethod.ToLower() == "cash"
                ? PaymentMethod.Cash
                : PaymentMethod.Pin;

            int ways = Math.Max(1, splitWays);
            decimal perPerson = Math.Round(total / ways, 2);

            for (int i = 0; i < ways; i++)
            {
                decimal amount = i == ways - 1 ? total - perPerson * (ways - 1) : perPerson;
                AddPayment(new Payment
                {
                    PaymentMethod = method,
                    Amount = amount,
                    Bill = new Bill { BillId = billId }
                });
            }

            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Paid);
            _tableRepository.ClearTable(tableId);
        }

        public List<Payment> GetAllPayments()
        {
            return _paymentRepository.GetAllPayments();
        }

        public List<Payment> GetPaymentsByBillId(int billId)
        {
            return _paymentRepository.GetPaymentsByBillId(billId);
        }

        public Payment GetPaymentById(int id)
        {
            return _paymentRepository.GetPaymentById(id);
        }

        public void AddPayment(Payment payment)
        {
            payment.Status = BillStatus.Paid;
            payment.PaidAt = DateTime.Now;

            _paymentRepository.AddPayment(payment);

            UpdateBillStatus(payment.Bill?.BillId ?? 0);
        }

        public void UpdatePayment(Payment payment)
        {
            _paymentRepository.UpdatePayment(payment);

            UpdateBillStatus(payment.Bill?.BillId ?? 0);
        }

        public void DeletePayment(int id)
        {
            Payment payment = _paymentRepository.GetPaymentById(id);

            if (payment != null)
            {
                int billId = payment.Bill?.BillId ?? 0;

                _paymentRepository.DeletePayment(id);

                UpdateBillStatus(billId);
            }
        }

        private void UpdateBillStatus(int billId)
        {
            Bill bill = _billRepository.GetBillById(billId);

            if (bill == null)
            {
                return;
            }

            List<Payment> payments = _paymentRepository.GetPaymentsByBillId(billId);

            decimal totalPaid = 0;
            foreach (Payment payment in payments)
                totalPaid += payment.Amount;

            if (totalPaid >= bill.Amount)
            {
                bill.Status = BillStatus.Paid;
            }
            else if (totalPaid > 0)
            {
                bill.Status = BillStatus.Partial;
            }
            else
            {
                bill.Status = BillStatus.Unpaid;
            }

            _billRepository.UpdateBill(bill);
        }
    }
}
