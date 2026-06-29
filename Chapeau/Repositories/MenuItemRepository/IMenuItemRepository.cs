using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IMenuItemRepository
    {
        List<MenuItem> GetAllMenuItems();
        MenuItem GetMenuItemById(int id);
    }
}
