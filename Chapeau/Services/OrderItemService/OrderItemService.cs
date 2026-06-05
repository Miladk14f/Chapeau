using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;

        public OrderItemService(IOrderItemRepository repository)
        {
            _repository = repository;
        }

        public List<OrderItem> GetAllOrderItems()
        {
            return _repository.GetAllOrderItems();
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            return _repository.GetOrderItemsByOrderId(orderId);
        }

        public OrderItem GetOrderItemById(int id)
        {
            return _repository.GetOrderItemById(id);
        }

        public void AddOrderItem(OrderItem item)
        {
            item.CreatedAt = DateTime.Now;
            _repository.AddOrderItem(item);
        }

        public void UpdateOrderItem(OrderItem item)
        {
            _repository.UpdateOrderItem(item);
        }

        public void DeleteOrderItem(int id)
        {
            _repository.DeleteOrderItem(id);
        }

        public void DeleteOrderItemsByOrderId(int orderId)
        {
            _repository.DeleteOrderItemsByOrderId(orderId);
        }
    }
}
