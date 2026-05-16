namespace Chapeau.ViewModels
{
    public class ManagerDashboardViewModel
    {
        public string ActiveTab { get; set; } = "revenue";

        // Stat cards
        public decimal RevenuePaid      { get; set; }
        public decimal RevenueOpen      { get; set; }
        public int     CoversToday      { get; set; }
        public int     CoversSeated     { get; set; }
        public decimal TipsReceived     { get; set; }
        public int     TablesCheckedOut { get; set; }
        public int     StockOutCount    { get; set; }
        public int     StockLowCount    { get; set; }

        // Revenue tab
        public List<CategoryRevenue>    CategoryRevenues    { get; set; } = new();
        public List<StaffPerformance>   StaffPerformances   { get; set; } = new();

        // VAT tab
        public decimal Vat9ExclBtw  { get; set; }
        public decimal Vat9Amount   { get; set; }
        public decimal Vat21ExclBtw { get; set; }
        public decimal Vat21Amount  { get; set; }

        // Stock tab
        public string              StockFilter  { get; set; } = "all";
        public List<StockItemRow>  StockItems   { get; set; } = new();

        // Feedback tab
        public int                 CommentCount    { get; set; }
        public int                 ComplaintCount  { get; set; }
        public int                 PraiseCount     { get; set; }
        public List<FeedbackRow>   FeedbackItems   { get; set; } = new();

        // Orders tab
        public List<OpenTableRow>  OpenTables { get; set; } = new();
    }

    public class CategoryRevenue
    {
        public string  Category { get; set; }
        public decimal Amount   { get; set; }
    }

    public class StaffPerformance
    {
        public string  Name     { get; set; }
        public string  Initials { get; set; }
        public int     Items    { get; set; }
        public decimal Revenue  { get; set; }
    }

    public class StockItemRow
    {
        public string  Name     { get; set; }
        public string  Category { get; set; }
        public decimal Price    { get; set; }
        public int     Vat      { get; set; }
        public int     Quantity { get; set; }
        public bool    IsLow    { get; set; }
        public bool    IsOut    { get; set; }
    }

    public class FeedbackRow
    {
        public string   Type      { get; set; }
        public int      TableId   { get; set; }
        public string   Text      { get; set; }
        public string   Staff     { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OpenTableRow
    {
        public int     TableId   { get; set; }
        public int     Guests    { get; set; }
        public string  WaiterName { get; set; }
        public int     ItemCount  { get; set; }
        public decimal Total      { get; set; }
    }
}
