using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IRestaurantTableService
    {
        List<RestaurantTable> GetAll();
        RestaurantTable GetById(int id);
        void SeatGuests(int tableId, int guests, int waiterId);
        void FreeTable(int tableId);
        void UpdateStatus(int tableId, string status);
    }
}
