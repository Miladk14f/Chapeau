using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class BillViewModel
    {
        public int TableId { get; set; }
        public int OrderId { get; set; }
        public int Guests { get; set; }
        public string WaiterName { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; } = new();

        public List<SplitPersonState> SplitPersons { get; set; }
    }
    public class PaymentConfirmationViewModel
    {
        public int TableId { get; set; }
        public int OrderId { get; set; }
        public int Guests { get; set; }
        public string WaiterName { get; set; }
        public DateTime PaidAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
        public decimal Tip { get; set; }

        public List<Payment> Payments { get; set; } = new();
        public bool IsSplit { get; set; }
    }

    public class BillReceiptViewModel
    {
        public List<OrderItem> Items { get; set; } = new();
        public decimal Tip { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int TableId { get; set; }
        public int Guests { get; set; }
        public bool Interactive { get; set; }
    }

    public class PersonPaymentInput
    {
        public decimal Amount { get; set; }
        public decimal Tip { get; set; }
        public string PaymentMethod { get; set; }
        public string FeedbackType { get; set; }
        public string FeedbackText { get; set; }
    }

    public class SplitPersonState
    {
        public int Index { get; set; }
        public decimal Amount { get; set; }
        public decimal Tip { get; set; }
        public string PaymentMethod { get; set; }
        public string FeedbackType { get; set; }
        public string FeedbackText { get; set; }
        public bool Paid { get; set; }

        public decimal Total => Amount + Tip;
    }

    public class SplitData
    {
        public int BillId { get; set; }
        public List<SplitPersonState> Persons { get; set; } = new();

        public bool AllPaid => Persons.All(p => p.Paid);
    }

}
