namespace Chapeau.Models
{
    public class StockItem
    {
        public int StockItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }

        public StockItem() { }

        public StockItem(int stockItemId, string name, int quantity, int maxQuantity)
        {
            StockItemId = stockItemId;
            Name = name;
            Quantity = quantity;
            MaxQuantity = maxQuantity;
        }
    }
}
