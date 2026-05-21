using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Tip { get; set; }
        public ESplitMethod SplitedMethod { get; set; }
        public decimal Amount { get; set; }
        public EBillStatus Status { get; set; }

        public Order? Order { get; set; }
        public List<Payment>? Payments { get; set; }

        public Bill() { }

        public Bill(int id, int orderId, decimal tip, ESplitMethod splitedMethod, decimal amount, EBillStatus status)
        {
            Id = id;
            OrderId = orderId;
            Tip = tip;
            SplitedMethod = splitedMethod;
            Amount = amount;
            Status = status;
        }
    }
}
