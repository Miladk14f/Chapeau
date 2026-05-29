using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        List<Order> GetOrdersByTableId(int tableId);
        List<Order> GetOrdersByStaffId(int staffId);
        Order GetOrderById(int orderId);
        int AddOrder(Order order);
        void UpdateOrder(Order order);
        void UpdateOrderStatus(int orderId, EOrderStatus status);
        void DeleteOrder(int orderId);
    }
}
