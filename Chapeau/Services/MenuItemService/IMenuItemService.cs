using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        List<MenuItem> GetAll();
        List<MenuItem> GetByCategory(string category);
        MenuItem GetById(int id);
        void Add(MenuItem item);
        void Update(MenuItem item);
        void Delete(int id);
        void UpdateStock(int id, bool inStock);
    }
}
