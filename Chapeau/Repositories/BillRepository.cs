namespace Chapeau.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Tip { get; set; }
        public string SplitedMethod { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }

        public Bill() { }

        public Bill(int id, int orderId, decimal tip, string splitedMethod, decimal amount, string status)
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
