using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public int Vat { get; set; }
        public EItemType ItemType { get; set; }

        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }

        public OrderItem() { }

        public OrderItem(int id, int orderId, int menuId, string name, int qty, decimal price, int vat, EItemType itemType)
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
