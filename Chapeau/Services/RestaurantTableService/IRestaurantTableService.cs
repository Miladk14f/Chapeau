using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IRestaurantTableService
    {
        List<RestaurantTable> GetAvailableTables();
        TableOverviewViewModel GetTableOverview();
        RestaurantTable GetTableById(int id);
        void SeatGuestsAtTable(int tableId, int guests, int waiterId);
        void ClearTable(int tableId);
        void ReserveTable(int tableId, string reservationName, int guests, DateTime reservationAt);
    }
}
