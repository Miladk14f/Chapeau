using Chapeau.Models;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public interface IRestaurantTableRepository
    {
        List<RestaurantTable> GetAll();
        RestaurantTable GetById(int id);
        void SeatGuests(int tableId, int guests, int waiterId);
        void FreeTable(int tableId);
        void UpdateStatus(int tableId, ETableStatus status);
    }
}
