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


            Order activeOrder = _orderRepository.GetOrdersByTableId(tableId)
                .Where(o => o.Status != OrderStatus.Paid)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (activeOrder == null)
                return null;

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(activeOrder.OrderId);

            return new BillViewModel
            {
                TableId = tableId,
                OrderId = activeOrder.OrderId,
                Guests = table.Guests ?? 0,
                WaiterName = GetWaiterName(table),
                GeneratedAt = DateTime.Now,
                Items = items
            };
        }

        public void ProcessPayment(int tableId, int orderId, List<PersonPaymentInput> persons)
        {
            decimal totalTip = persons.Sum(p => p.Tip);

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(orderId);
            decimal itemsTotal = items.Sum(item => item.Price * item.Qty);

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
                AddPayment(new Payment
                {
                    PaymentMethod = ParsePaymentMethod(person.PaymentMethod),
                    Amount = person.Amount + person.Tip,
                    Bill = new Bill { BillId = billId }
                });
            }

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


            Bill bill = _billRepository.GetAllBills()
                .Where(b => b.Order != null && b.Order.OrderId == orderId)
                .OrderByDescending(b => b.BillId)
                .FirstOrDefault();

            List<Payment> payments = bill != null ? _paymentRepository.GetPaymentsByBillId(bill.BillId) : new List<Payment>();
            decimal tip = bill?.Tip ?? 0;
            DateTime? paidAt = payments.Count > 0 ? payments.Max(p => p.PaidAt) : null;

            return new PaymentConfirmationViewModel
            {
                TableId = tableId,
                OrderId = orderId,
                Guests = table?.Guests ?? 0,
                WaiterName = GetWaiterName(table),
                PaidAt = paidAt ?? DateTime.Now,
                Items = items,
                Tip = tip,
                Payments = payments,
                IsSplit = payments.Count > 1
            };
        }

        public SplitData StartSplitBill(int orderId, List<PersonPaymentInput> persons)
        {
            decimal totalTip = persons.Sum(p => p.Tip);
            decimal total = persons.Sum(p => p.Amount + p.Tip);

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


        private string GetWaiterName(RestaurantTable table)
        {
            if (table?.Waiter == null)
                return "";
            Staff waiter = _staffRepository.GetAllStaff()
                .FirstOrDefault(s => s.StaffId == table.Waiter.StaffId);
            return waiter?.Name ?? "";
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
