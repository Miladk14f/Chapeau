using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        List<MenuItem> GetAllMenuItems();
        List<MenuItem> GetMenuItemsByCategory(string category);
        MenuItem GetMenuItemById(int id);
        void AddMenuItem(MenuItem item);
        void UpdateMenuItem(MenuItem item);
        void DeleteMenuItem(int id);
        void UpdateMenuItemStock(int id, bool inStock);
    }
}
