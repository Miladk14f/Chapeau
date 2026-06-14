using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public decimal Tip { get; set; }
        public SplitMethod SplitedMethod { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }

        public Order? Order { get; set; }
        public List<Payment>? Payments { get; set; }

        public Bill() { }

        public Bill(int billId, decimal tip, SplitMethod splitedMethod, decimal amount, BillStatus status)
        {
            BillId = billId;
            Tip = tip;
            SplitedMethod = splitedMethod;
            Amount = amount;
            Status = status;
        }
    }
}
