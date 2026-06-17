namespace Chapeau.ViewModels
{
    public class PersonPaymentInput
    {
        public decimal Amount { get; set; }
        public decimal Tip { get; set; }
        public string PaymentMethod { get; set; } = "pin";
        public string FeedbackType { get; set; }
        public string FeedbackText { get; set; }
    }

    public class SplitPersonState
    {
        public int Index { get; set; }
        public decimal Amount { get; set; }
        public decimal Tip { get; set; }
        public string PaymentMethod { get; set; } = "pin";
        public string FeedbackType { get; set; }
        public string FeedbackText { get; set; }
        public bool Paid { get; set; }

        public decimal Total => Amount + Tip;
    }

    public class SplitData
    {
        public int BillId { get; set; }
        public List<SplitPersonState> Persons { get; set; } = new();
    }

    public class BillViewModel
    {
        public int TableId { get; set; }
        public int OrderId { get; set; }
        public int Guests { get; set; }
        public string WaiterName { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public List<BillItemRow> Items9 { get; set; } = new();
        public List<BillItemRow> Items21 { get; set; } = new();

        public decimal Excl9 { get; set; }
        public decimal Vat9Amount { get; set; }
        public decimal Excl21 { get; set; }
        public decimal Vat21Amount { get; set; }

        public decimal Subtotal => Excl9 + Vat9Amount + Excl21 + Vat21Amount;
        public decimal TotalVat => Vat9Amount + Vat21Amount;
        public decimal TotalToPay => Subtotal;

        // Split-in-progress state (null when no active split)
        public List<SplitPersonState> SplitPersons { get; set; }
        public int SplitBillId { get; set; }
    }

    public class BillItemRow
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Qty;
        public string StaffName { get; set; }
        public int Vat { get; set; }
    }
}
