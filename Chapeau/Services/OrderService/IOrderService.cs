using Chapeau.Models.Enums;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        OrderViewModel GetOrderPage(int tableId, int staffId, string category = null, string subCategory = null);
        List<PreparationCard> GetPreparationCards(SubCategory[] types, int warningMinutes, int urgentMinutes);
        List<HistoryCard> GetOrderHistory(SubCategory[] types);

        void AddItemToTable(int menuItemId, int tableId);
        void DecreaseItemForTable(int orderItemId, int tableId);
        void RemoveItemFromTable(int orderItemId, int tableId);
        void UpdateOrderItemNote(int id, string note);
        void UpdateOrderItemStatus(int id, OrderItemStatus status);
        void ServeOrderItem(int orderItemId);
        void StartPreparingItems(int orderId, SubCategory[] types);
        void MarkOrderItemsReady(int orderId, SubCategory[] types);
        void DeleteOrder(int tableId);
    }
}
