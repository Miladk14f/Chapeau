using Chapeau.Models;
using Chapeau.Models.Enums;

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
        void UpdateOrderItemStatus(int id, OrderItemStatus status);
        void UpdateOrderItemNote(int id, string note);
        void MarkOrderItemsReady(int orderId, ItemType type);
        void DeleteOrderItem(int id);
        void DeleteOrderItemsByOrderId(int orderId);
        void AddItemToTable(int menuItemId, int tableId);
        void DecreaseItemForTable(int orderItemId, int tableId);
        void RemoveItemFromTable(int orderItemId, int tableId);
        void ServeOrderItem(int orderItemId);
    }
}
