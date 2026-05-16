using System.Collections.Generic;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class StockItemService
    {
        private readonly IStockItemRepository _stockRepo;

        public StockItemService(IStockItemRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public List<StockItem> GetInventory()
        {
            return _stockRepo.GetAllStockItems();
        }

        public void ChangeStockLevel(int itemId, int newQuantity)
        {
            _stockRepo.UpdateStockQuantity(itemId, newQuantity);
        }
    }
}