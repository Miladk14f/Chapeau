using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IOrderItemService
    {
        List<OrderItem> GetAllOrderItems();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        List<OrderItem> GetOrderItemsByTableId(int tableId);
        OrderItem GetOrderItemById(int id);
        void AddOrderItem(OrderItem item);
        void UpdateOrderItem(OrderItem item);
        void DeleteOrderItem(int id);
        void DeleteOrderItemsByOrderId(int orderId);
    }
}
