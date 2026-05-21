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

        public List<RestaurantTable> GetAll()
        {
            return _repository.GetAll();
        }

        public RestaurantTable GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void SeatGuests(int tableId, int guests, int waiterId)
        {
            _repository.SeatGuests(tableId, guests, waiterId);
        }

        public void FreeTable(int tableId)
        {
            _repository.FreeTable(tableId);
        }

        public void UpdateStatus(int tableId, ETableStatus status)
        {
            _repository.UpdateStatus(tableId, status);
        }
    }
}
