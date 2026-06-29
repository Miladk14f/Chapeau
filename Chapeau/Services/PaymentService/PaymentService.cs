using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBillService _billService;
        private readonly ICommentService _commentService;
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantTableRepository _tableRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBillService billService,
            ICommentService commentService,
            IOrderRepository orderRepository,
            IRestaurantTableRepository tableRepository)
        {
            _paymentRepository = paymentRepository;
            _billService = billService;
            _commentService = commentService;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
        }

        public void ProcessPayment(int tableId, int orderId, List<PersonPaymentInput> persons)
        {
            decimal totalTip = persons.Sum(p => p.Tip);

            List<OrderItem> items = _orderRepository.GetOrderItemsByOrderId(orderId);
            decimal itemsTotal = items.Sum(item => item.Price * item.Qty);
            decimal billTotal = itemsTotal + totalTip;

            int billId = _billService.CreateBill(orderId, billTotal, totalTip, persons.Count > 1);

            foreach (PersonPaymentInput person in persons)
            {
                AddPayment(new Payment
                {
                    PaymentMethod = ParsePaymentMethod(person.PaymentMethod),
                    Amount = person.Amount + person.Tip,
                    Bill = new Bill { BillId = billId }
                });

                SaveFeedback(tableId, person.FeedbackType, person.FeedbackText);
            }

            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Paid);
        }

        public SplitData StartSplitBill(int orderId, List<PersonPaymentInput> persons)
        {
            decimal totalTip = persons.Sum(p => p.Tip);
            decimal total = persons.Sum(p => p.Amount + p.Tip);

            int billId = _billService.CreateBill(orderId, total, totalTip, true);

            SplitData split = new SplitData { BillId = billId };
            for (int i = 0; i < persons.Count; i++)
            {
                PersonPaymentInput p = persons[i];
                split.Persons.Add(new SplitPersonState
                {
                    Index = i,
                    Amount = p.Amount,
                    Tip = p.Tip,
                    PaymentMethod = p.PaymentMethod ?? "debit",
                    FeedbackType = p.FeedbackType ?? "Comment",
                    FeedbackText = p.FeedbackText ?? "",
                    Paid = false
                });
            }
            return split;
        }

        public void AddSplitPersonPayment(int tableId, int billId, decimal amount, string paymentMethod, string feedbackType, string feedbackText)
        {
            AddPayment(new Payment
            {
                PaymentMethod = ParsePaymentMethod(paymentMethod),
                Amount = amount,
                Bill = new Bill { BillId = billId }
            });

            SaveFeedback(tableId, feedbackType, feedbackText);
        }

        private void SaveFeedback(int tableId, string feedbackType, string feedbackText)
        {
            if (!string.IsNullOrWhiteSpace(feedbackText))
                _commentService.AddCommentForTable(tableId, feedbackType ?? "Comment", feedbackText);
        }

        public void CompleteOrder(int orderId)
        {
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Paid);
        }

        public void CloseTable(int tableId, int orderId)
        {
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Paid);
            _tableRepository.ClearTable(tableId);
        }

        private void AddPayment(Payment payment)
        {
            payment.Status = BillStatus.Paid;
            payment.PaidAt = DateTime.Now;

            _paymentRepository.AddPayment(payment);

            _billService.UpdateBillStatus(payment.Bill?.BillId ?? 0);
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
    }
}
