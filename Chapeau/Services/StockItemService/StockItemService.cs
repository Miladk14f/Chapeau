using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class StockItemService : IStockItemService
    {
        private readonly IStockItemRepository _repository;

        public StockItemService(IStockItemRepository repository)
        {
            _repository = repository;
        }

        public List<StockItem> GetAll()
        {
            return _repository.GetAll();
        }

        public StockItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Add(StockItem item)
        {
            _repository.Add(item);
        }

        public void Update(StockItem item)
        {
            _repository.Update(item);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void UpdateQuantity(int id, int quantity)
        {
            _repository.UpdateQuantity(id, quantity);
        }
    }
}
