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

        public List<StockItem> GetAllStockItems()
        {
            return _repository.GetAllStockItems();
        }

        public StockItem GetStockItemById(int id)
        {
            return _repository.GetStockItemById(id);
        }

        public void AddStockItem(StockItem item)
        {
            _repository.AddStockItem(item);
        }

        public void UpdateStockItem(StockItem item)
        {
            _repository.UpdateStockItem(item);
        }

        public void DeleteStockItem(int id)
        {
            _repository.DeleteStockItem(id);
        }

        public void UpdateStockItemQuantity(int id, int quantity)
        {
            _repository.UpdateStockItemQuantity(id, quantity);
        }
    }
}
