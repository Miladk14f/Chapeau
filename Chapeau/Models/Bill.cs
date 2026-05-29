using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public decimal Tip { get; set; }
        public ESplitMethod SplitedMethod { get; set; }
        public decimal Amount { get; set; }
        public EBillStatus Status { get; set; }

        public Order? Order { get; set; }
        public List<Payment>? Payments { get; set; }

        public Bill() { }

        public Bill(int billId, decimal tip, ESplitMethod splitedMethod, decimal amount, EBillStatus status)
        {
            BillId = billId;
            Tip = tip;
            SplitedMethod = splitedMethod;
            Amount = amount;
            Status = status;
        }
    }
}
