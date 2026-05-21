using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;

        public StaffService(IStaffRepository repository)
        {
            _repository = repository;
        }

        public List<Staff> GetAll()
        {
            return _repository.GetAll();
        }

        public Staff GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Staff Login(string name, string pin)
        {
            return _repository.GetByCredentials(name, pin);
        }

        public void Add(Staff staff)
        {
            _repository.Add(staff);
        }

        public void Update(Staff staff)
        {
            _repository.Update(staff);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
