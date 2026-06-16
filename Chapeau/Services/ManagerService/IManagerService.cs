using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public interface IManagerService
    {
        ManagerDashboardViewModel GetDashboard(string tab, string stockFilter);
    }
}
