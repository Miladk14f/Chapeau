using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetAll();
        Staff GetById(int id);
        Staff GetByCredentials(string name, string pin);
        void Add(Staff staff);
        void Update(Staff staff);
        void Delete(int id);
    }
}
