using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Vat { get; set; }
        public ItemType ItemType { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderItemStatus Status { get; set; }
        public string Note { get; set; }

        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }

        public OrderItem() { }

        public OrderItem(int orderItemId, string name, int qty, decimal price, int vat, ItemType itemType)
        {
            OrderItemId = orderItemId;
            Name = name;
            Qty = qty;
            Price = price;
            Vat = vat;
            ItemType = itemType;
        }
    }
}
