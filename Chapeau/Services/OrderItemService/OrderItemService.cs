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

        public List<OrderItem> GetByOrderId(int orderId)
        {
            return _repository.GetByOrderId(orderId);
        }

        public OrderItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Add(OrderItem item)
        {
            _repository.Add(item);
        }

        public void Update(OrderItem item)
        {
            _repository.Update(item);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void DeleteByOrderId(int orderId)
        {
            _repository.DeleteByOrderId(orderId);
        }
    }
}
