using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IStockItemService
    {
        List<StockItem> GetAllStockItems();
        StockItem GetStockItemById(int id);
        void AddStockItem(StockItem item);
        void UpdateStockItem(StockItem item);
        void DeleteStockItem(int id);
        void UpdateStockItemQuantity(int id, int quantity);
    }
}
