using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IStockItemRepository
    {
        List<StockItem> GetAll();
        StockItem GetById(int id);
        void Add(StockItem item);
        void Update(StockItem item);
        void Delete(int id);
        void UpdateQuantity(int id, int quantity);
    }
}
