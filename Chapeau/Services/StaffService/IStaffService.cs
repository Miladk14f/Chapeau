using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IStaffService
    {
        List<Staff> GetAllStaff();
        List<Staff> GetAllStaffWithoutPins();
        Staff GetStaffById(int id);
        Staff TryLogin(int staffId, string password);
        void CreateStaff(string name, string role, string pin);
        bool UpdateStaffDetails(int id, string name, string role, string pin);
        void DeleteStaff(int id);
    }
}
