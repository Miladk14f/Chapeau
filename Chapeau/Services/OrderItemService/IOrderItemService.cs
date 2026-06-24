using Chapeau.Models.Enums;

namespace Chapeau.Services
{
    public interface IOrderItemService
    {
        void AddItemToTable(int menuItemId, int tableId);
        void DecreaseItemForTable(int orderItemId, int tableId);
        void RemoveItemFromTable(int orderItemId, int tableId);
        void UpdateOrderItemNote(int id, string note);
        void UpdateOrderItemStatus(int id, OrderItemStatus status);
        void ServeOrderItem(int orderItemId);
        void StartPreparingItems(int orderId, ItemType[] types);
        void MarkOrderItemsReady(int orderId, ItemType[] types);
    }
}
