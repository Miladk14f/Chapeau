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

        public List<MenuItem> GetAll()
        {
            return _repository.GetAll();
        }

        public List<MenuItem> GetByCategory(string category)
        {
            return _repository.GetByCategory(category);
        }

        public MenuItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Add(MenuItem item)
        {
            _repository.Add(item);
        }

        public void Update(MenuItem item)
        {
            _repository.Update(item);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void UpdateStock(int id, bool inStock)
        {
            _repository.UpdateStock(id, inStock);
        }
    }
}
