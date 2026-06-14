using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();
        List<Order> GetOrdersByTableId(int tableId);
        List<Order> GetOrdersByStaffId(int staffId);
        Order GetOrderById(int orderId);
        int AddOrder(Order order);
        void UpdateOrder(Order order);
        void UpdateOrderStatus(int orderId, OrderStatus status);
        void DeleteOrder(int orderId);
    }
}
