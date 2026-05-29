using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public EOrderStatus Status { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public RestaurantTable? Table { get; set; }
        public Staff? Staff { get; set; }
        public List<OrderItem>? Items { get; set; }

        public Order() { }

        public Order(int orderId, EOrderStatus status, string note, DateTime createdAt, decimal totalPrice)
        {
            OrderId = orderId;
            Status = status;
            Note = note;
            CreatedAt = createdAt;
            TotalPrice = totalPrice;
        }
    }
}
