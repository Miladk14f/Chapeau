using Chapeau.Models.Enums;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        OrderViewModel GetOrderPage(int tableId, int staffId);
        List<PreparationCard> GetPreparationCards(ItemType[] types, int warningMinutes, int urgentMinutes);
        List<HistoryCard> GetOrderHistory(ItemType[] types);
    }
}
