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

        public List<MenuItem> GetAllMenuItems()
        {
            return _repository.GetAllMenuItems();
        }

        public List<MenuItem> GetMenuItemsByCategory(string category)
        {
            return _repository.GetMenuItemsByCategory(category);
        }

        public MenuItem GetMenuItemById(int id)
        {
            return _repository.GetMenuItemById(id);
        }

        public void AddMenuItem(MenuItem item)
        {
            _repository.AddMenuItem(item);
        }

        public void UpdateMenuItem(MenuItem item)
        {
            _repository.UpdateMenuItem(item);
        }

        public void DeleteMenuItem(int id)
        {
            _repository.DeleteMenuItem(id);
        }

        public void UpdateMenuItemStock(int id, bool inStock)
        {
            _repository.UpdateMenuItemStock(id, inStock);
        }
    }
}
