using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();
        List<Order> GetOrdersByTableId(int tableId);
        Order GetActiveOrderByTableId(int tableId);
        List<Order> GetOrdersByStaffId(int staffId);
        Order GetOrderById(int orderId);
        int AddOrder(Order order);
        void UpdateOrder(Order order);
        void UpdateOrderStatus(int orderId, OrderStatus status);
        void DeleteOrder(int orderId);

        List<OrderItem> GetAllOrderItems();
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        List<OrderItem> GetOrderItemsByTableId(int tableId);
        OrderItem GetOrderItemById(int id);
        void AddOrderItem(OrderItem item);
        void UpdateOrderItem(OrderItem item);
        void UpdateOrderItemStatus(int id, OrderItemStatus status);
        void UpdateOrderItemNote(int id, string note);
        void UpdateOrderItemsStatusByType(int orderId, SubCategory[] types, OrderItemStatus fromStatus, OrderItemStatus toStatus);
        void DeleteOrderItem(int id);
        void DeleteOrderItemsByOrderId(int orderId);
    }
}
