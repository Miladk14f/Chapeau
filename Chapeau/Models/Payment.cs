using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public EPaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public EBillStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }

        public Bill? Bill { get; set; }

        public Payment() { }

        public Payment(int id, int billId, EPaymentMethod paymentMethod, decimal amount, EBillStatus status, DateTime? paidAt)
        {
            Id = id;
            BillId = billId;
            PaymentMethod = paymentMethod;
            Amount = amount;
            Status = status;
            PaidAt = paidAt;
        }
    }
}
