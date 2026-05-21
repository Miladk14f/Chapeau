namespace Chapeau.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }

        public Payment() { }

        public Payment(int id, int billId, string paymentMethod, decimal amount, string status, DateTime? paidAt)
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
