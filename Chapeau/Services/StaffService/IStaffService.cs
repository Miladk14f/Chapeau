using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IStaffService
    {
        List<Staff> GetAll();
        Staff GetById(int id);
        Staff Login(string name, string pin);
        void Add(Staff staff);
        void Update(Staff staff);
        void Delete(int id);
    }
}
