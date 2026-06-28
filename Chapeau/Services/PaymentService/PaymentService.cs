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
        private readonly IRestaurantTableRepository _tableRepository;
        private readonly IStaffRepository _staffRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBillRepository billRepository,
            IOrderRepository orderRepository,
            IRestaurantTableRepository tableRepository,
            IStaffRepository staffRepository)
        {
            _paymentRepository = paymentRepository;
            _billRepository = billRepository;
            _orderRepository = orderRepository;
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

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(activeOrder.OrderId);
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

        public void ProcessPayment(int tableId, int orderId, List<PersonPaymentInput> persons)
        {
            decimal totalTip = 0;
            foreach (PersonPaymentInput p in persons)
                totalTip += p.Tip;

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(orderId);
            decimal itemsTotal = 0;
            foreach (OrderItem item in items)
                itemsTotal += item.Price * item.Qty;

            decimal billTotal = itemsTotal + totalTip;
            SplitMethod split = persons.Count > 1 ? SplitMethod.Equal : SplitMethod.None;

            Bill bill = new Bill
            {
                Tip = totalTip,
                SplitedMethod = split,
                Amount = billTotal,
                Status = BillStatus.Unpaid,
                Order = new Order { OrderId = orderId }
            };
            int billId = _billRepository.AddBill(bill);

            foreach (PersonPaymentInput person in persons)
            {
                PaymentMethod method = ParsePaymentMethod(person.PaymentMethod);

                AddPayment(new Payment
                {
                    PaymentMethod = method,
                    Amount = person.Amount + person.Tip,
                    Bill = new Bill { BillId = billId }
                });
            }

            // mark paid but keep table open — cleared later from the confirmation page
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Paid);
        }

        public void CompleteOrder(int orderId)
        {
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Paid);
        }

        public PaymentConfirmationViewModel GetConfirmation(int orderId)
        {
            Order order = _orderRepository.GetOrderById(orderId);
            if (order == null)
                return null;

            int tableId = order.Table != null ? order.Table.TableId : 0;
            RestaurantTable table = _tableRepository.GetTableById(tableId);
            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(orderId);
            List<Staff> staff = _staffRepository.GetAllStaff();

            int orderStaffId = order.Staff != null ? order.Staff.StaffId : 0;
            string orderStaffName = "";
            foreach (Staff s in staff)
                if (s.StaffId == orderStaffId) { orderStaffName = s.Name; break; }

            string waiterName = "";
            if (table?.Waiter != null)
                foreach (Staff s in staff)
                    if (s.StaffId == table.Waiter.StaffId) { waiterName = s.Name; break; }

            List<BillItemRow> items9 = new List<BillItemRow>();
            List<BillItemRow> items21 = new List<BillItemRow>();
            decimal sum9 = 0, sum21 = 0;
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
                if (item.Vat == 9) { items9.Add(row); sum9 += lineTotal; }
                else if (item.Vat == 21) { items21.Add(row); sum21 += lineTotal; }
            }

            decimal excl9 = sum9 > 0 ? Math.Round(sum9 / 1.09m, 2) : 0;
            decimal excl21 = sum21 > 0 ? Math.Round(sum21 / 1.21m, 2) : 0;

            // latest bill for this order + its payments
            Bill bill = null;
            foreach (Bill b in _billRepository.GetAllBills())
                if (b.Order != null && b.Order.OrderId == orderId && (bill == null || b.BillId > bill.BillId))
                    bill = b;

            List<Payment> payments = bill != null ? _paymentRepository.GetPaymentsByBillId(bill.BillId) : new List<Payment>();
            decimal tip = bill?.Tip ?? 0;
            DateTime? paidAt = null;
            foreach (Payment p in payments)
                if (p.PaidAt != null && (paidAt == null || p.PaidAt > paidAt)) paidAt = p.PaidAt;

            return new PaymentConfirmationViewModel
            {
                TableId = tableId,
                OrderId = orderId,
                Guests = table?.Guests ?? 0,
                WaiterName = waiterName,
                PaidAt = paidAt ?? DateTime.Now,
                Items9 = items9,
                Items21 = items21,
                Excl9 = excl9,
                Vat9Amount = Math.Round(sum9 - excl9, 2),
                Excl21 = excl21,
                Vat21Amount = Math.Round(sum21 - excl21, 2),
                Tip = tip,
                Payments = payments,
                IsSplit = payments.Count > 1
            };
        }

        public SplitData StartSplitBill(int orderId, List<PersonPaymentInput> persons)
        {
            decimal totalTip = 0;
            decimal total = 0;
            foreach (PersonPaymentInput p in persons)
            {
                total += p.Amount + p.Tip;
                totalTip += p.Tip;
            }

            Bill bill = new Bill
            {
                Tip = totalTip,
                SplitedMethod = SplitMethod.Equal,
                Amount = total,
                Status = BillStatus.Unpaid,
                Order = new Order { OrderId = orderId }
            };
            int billId = _billRepository.AddBill(bill);

            SplitData split = new SplitData { BillId = billId };
            for (int i = 0; i < persons.Count; i++)
            {
                PersonPaymentInput p = persons[i];
                split.Persons.Add(new SplitPersonState
                {
                    Index = i,
                    Amount = p.Amount,
                    Tip = p.Tip,
                    PaymentMethod = p.PaymentMethod ?? "pin",
                    FeedbackType = p.FeedbackType ?? "Comment",
                    FeedbackText = p.FeedbackText ?? "",
                    Paid = false
                });
            }
            return split;
        }

        public void AddSplitPersonPayment(int billId, decimal amount, string paymentMethod)
        {
            PaymentMethod method = ParsePaymentMethod(paymentMethod);
            AddPayment(new Payment
            {
                PaymentMethod = method,
                Amount = amount,
                Bill = new Bill { BillId = billId }
            });
        }

        public void CloseTable(int tableId, int orderId)
        {
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

        private static PaymentMethod ParsePaymentMethod(string method)
        {
            switch (method?.ToLower())
            {
                case "cash": return PaymentMethod.Cash;
                case "credit":
                case "creditcard": return PaymentMethod.CreditCard;
                case "debit":
                case "debitcard": return PaymentMethod.DebitCard;
                default: return PaymentMethod.DebitCard;
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
