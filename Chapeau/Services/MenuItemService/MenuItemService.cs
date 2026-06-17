using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _repository;

        public MenuItemService(IMenuItemRepository repository)
        {
            _repository = repository;
        }

        public List<MenuItem> GetAllMenuItems() => _repository.GetAllMenuItems();
    }
}
