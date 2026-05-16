using System.Collections.Generic;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IStockItemRepository
    {
        List<StockItem> GetAllStockItems();
        void UpdateStockQuantity(int itemId, int newQuantity);
    }
}