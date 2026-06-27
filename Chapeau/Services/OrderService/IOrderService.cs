using Chapeau.Models.Enums;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        OrderViewModel GetOrderPage(int tableId, int staffId);
        List<PreparationCard> GetPreparationCards(ItemType[] types, int warningMinutes, int urgentMinutes);
        List<HistoryCard> GetOrderHistory(ItemType[] types);

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
