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

        public List<OrderItem> GetOrderItemsByTableId(int orderId)
        {
            return _repository.GetOrderItemsByTableId(orderId);
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

        public void UpdateOrderItemStatus(int id, Models.Enums.OrderItemStatus status)
        {
            _repository.UpdateOrderItemStatus(id, status);
        }

        public void UpdateOrderItemNote(int id, string note)
        {
            _repository.UpdateOrderItemNote(id, note);
        }

        public void MarkOrderItemsReady(int orderId, Models.Enums.ItemType type)
        {
            _repository.MarkOrderItemsReady(orderId, type);
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
