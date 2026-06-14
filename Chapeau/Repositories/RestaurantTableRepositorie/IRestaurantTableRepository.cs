using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IRestaurantTableRepository
    {
        List<RestaurantTable> GetAllTables();
        RestaurantTable GetTableById(int id);
        void SeatGuestsAtTable(int tableId, int guests, int waiterId);
        void ClearTable(int tableId);
        void UpdateTableStatus(int tableId, TableStatus status);
        void ReserveTable(int tableId, string reservationName, int guests, DateTime reservationAt);
    }
}
