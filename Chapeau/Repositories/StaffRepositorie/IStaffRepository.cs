using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetAllStaff();
        Staff GetStaffById(int id);
        Staff GetStaffByCredentials(string name, string pin);
        void AddStaff(Staff staff);
        void UpdateStaff(Staff staff);
        void DeleteStaff(int id);
    }
}
