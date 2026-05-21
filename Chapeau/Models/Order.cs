using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int StaffId { get; set; }
        public EOrderStatus Status { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public RestaurantTable? Table { get; set; }
        public Staff? Staff { get; set; }
        public List<OrderItem>? Items { get; set; }

        public Order() { }

        public Order(int id, int tableId, int staffId, EOrderStatus status, string note, DateTime createdAt, decimal totalPrice)
        {
            Id = id;
            TableId = tableId;
            StaffId = staffId;
            Status = status;
            Note = note;
            CreatedAt = createdAt;
            TotalPrice = totalPrice;
        }
    }
}
