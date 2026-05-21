using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IOrderItemRepository
    {
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        OrderItem GetOrderItemById(int id);
        void AddOrderItem(OrderItem item);
        void UpdateOrderItem(OrderItem item);
        void DeleteOrderItem(int id);
        void DeleteOrderItemsByOrderId(int orderId);
    }
}
