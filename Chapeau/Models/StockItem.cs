namespace Chapeau.Models
{
    public class StockItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }

        public StockItem() { }

        public StockItem(int id, string name, int quantity, int maxQuantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            MaxQuantity = maxQuantity;
        }
    }
}
