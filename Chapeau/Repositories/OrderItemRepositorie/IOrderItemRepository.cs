using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IOrderItemRepository
    {
        List<OrderItem> GetAllOrderItems();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        List<OrderItem> GetOrderItemsByTableId(int tableId);
        OrderItem GetOrderItemById(int id);
        void AddOrderItem(OrderItem item);
        void UpdateOrderItem(OrderItem item);
        void UpdateOrderItemStatus(int id, OrderItemStatus status);
        void UpdateOrderItemNote(int id, string note);
        void UpdateOrderItemsStatusByType(int orderId, ItemType type, OrderItemStatus fromStatus, OrderItemStatus toStatus);
        void DeleteOrderItem(int id);
        void DeleteOrderItemsByOrderId(int orderId);
    }
}
