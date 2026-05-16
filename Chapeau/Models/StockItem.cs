namespace Chapeau.Models
{
    public class StockItem
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}
