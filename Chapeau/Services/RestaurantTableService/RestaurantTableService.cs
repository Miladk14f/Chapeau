using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IRestaurantTableRepository _repository;

        public RestaurantTableService(IRestaurantTableRepository repository)
        {
            _repository = repository;
        }

        public List<RestaurantTable> GetAllTables()
        {
            return _repository.GetAllTables();
        }

        public RestaurantTable GetTableById(int id)
        {
            return _repository.GetTableById(id);
        }

        public void SeatGuestsAtTable(int tableId, int guests, int waiterId)
        {
            _repository.SeatGuestsAtTable(tableId, guests, waiterId);
        }

        public void ClearTable(int tableId)
        {
            _repository.ClearTable(tableId);
        }

        public void UpdateTableStatus(int tableId, ETableStatus status)
        {
            _repository.UpdateTableStatus(tableId, status);
        }

        public void ReserveTable(int tableId, string reservationName, int guests, DateTime reservationAt)
        {
            _repository.ReserveTable(tableId, reservationName, guests, reservationAt);
        }
    }
}
