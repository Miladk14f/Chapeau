using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IStaffService
    {
        List<Staff> GetAllStaff();
        List<Staff> GetAllStaffWithoutPins();
        Staff GetStaffById(int id);
        Staff LoginStaff(string name, string pin);
        Staff TryLogin(int staffId, string password);
        void AddStaff(Staff staff);
        void CreateStaff(string name, string role, string pin);
        bool UpdateStaffDetails(int id, string name, string role, string pin);
        void UpdateStaff(Staff staff);
        void UpdateStaffInfo(Staff staff);
        void DeleteStaff(int id);
        bool VerifyStaffPin(Staff staff, string pin);
    }
}
