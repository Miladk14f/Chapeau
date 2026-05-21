using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IStaffService
    {
        List<Staff> GetAllStaff();
        Staff GetStaffById(int id);
        Staff LoginStaff(string name, string pin);
        void AddStaff(Staff staff);
        void UpdateStaff(Staff staff);
        void UpdateStaffInfo(Staff staff);
        void DeleteStaff(int id);
        bool VerifyStaffPin(Staff staff, string pin);
    }
}
