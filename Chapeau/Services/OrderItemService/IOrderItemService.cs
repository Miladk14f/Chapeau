using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IOrderItemService
    {
        List<OrderItem> GetByOrderId(int orderId);
        OrderItem GetById(int id);
        void Add(OrderItem item);
        void Update(OrderItem item);
        void Delete(int id);
        void DeleteByOrderId(int orderId);
    }
}
