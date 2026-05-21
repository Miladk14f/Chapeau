using System.Security.Cryptography;
using System.Text;
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
            string hashedPin = HashPin(pin);
            return _repository.GetByCredentials(name, hashedPin);
        }

        public void Add(Staff staff)
        {
            staff.Pin = HashPin(staff.Pin);
            _repository.Add(staff);
        }

        public void Update(Staff staff)
        {
            staff.Pin = HashPin(staff.Pin);
            _repository.Update(staff);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public bool VerifyPin(Staff staff, string pin)
        {
            return staff.Pin == HashPin(pin);
        }

        private string HashPin(string pin)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
