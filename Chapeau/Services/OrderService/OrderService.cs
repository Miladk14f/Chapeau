using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public List<Order> GetAllOrders()
        {
            return _repository.GetAllOrders();
        }

        public List<Order> GetOrdersByTableId(int tableId)
        {
            return _repository.GetOrdersByTableId(tableId);
        }

        public List<Order> GetOrdersByStaffId(int staffId)
        {
            return _repository.GetOrdersByStaffId(staffId);
        }

        public Order GetOrderById(int orderId)
        {
            return _repository.GetOrderById(orderId);
        }

        public int AddOrder(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.Status = EOrderStatus.Pending;
            return _repository.AddOrder(order);
        }

        public void UpdateOrder(Order order)
        {
            _repository.UpdateOrder(order);
        }

        public void UpdateOrderStatus(int orderId, EOrderStatus status)
        {
            _repository.UpdateOrderStatus(orderId, status);
        }

        public void DeleteOrder(int orderId)
        {
            _repository.DeleteOrder(orderId);
        }
    }
}
