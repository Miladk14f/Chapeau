using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        List<Order> GetOrdersByTableId(int tableId);
        List<Order> GetOrdersByStaffId(int staffId);
        Order GetOrderById(int orderId);
        Order GetActiveOrderByTableId(int tableId);
        int AddOrder(Order order);
        void UpdateOrder(Order order);
        void UpdateOrderStatus(int orderId, OrderStatus status);
        void DeleteOrder(int orderId);
        List<PreparationCard> GetPreparationCards(ItemType type, int warningMinutes, int urgentMinutes);
        OrderViewModel GetOrderPage(int tableId);
    }
}
