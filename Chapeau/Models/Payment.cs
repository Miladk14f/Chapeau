using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }

        public Bill? Bill { get; set; }

        public Payment() { }

        public Payment(int paymentId, PaymentMethod paymentMethod, decimal amount, BillStatus status, DateTime? paidAt)
        {
            PaymentId = paymentId;
            PaymentMethod = paymentMethod;
            Amount = amount;
            Status = status;
            PaidAt = paidAt;
        }
    }
}
