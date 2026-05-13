namespace Chapeau.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string MenuId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Vat { get; set; }
        public string ItemType { get; set; }

        public OrderItem() { }

        public OrderItem(int id, int orderId, string menuId, string name, int qty, decimal price, int vat, string itemType)
        {
            Id = id;
            OrderId = orderId;
            MenuId = menuId;
            Name = name;
            Qty = qty;
            Price = price;
            Vat = vat;
            ItemType = itemType;
        }
    }
}
