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

        public List<Staff> GetAllStaff()
        {
            return _repository.GetAllStaff();
        }

        public Staff GetStaffById(int id)
        {
            return _repository.GetStaffById(id);
        }

        public Staff LoginStaff(string name, string pin)
        {
            string hashedPin = HashPin(pin);
            return _repository.GetStaffByCredentials(name, hashedPin);
        }

        public void AddStaff(Staff staff)
        {
            staff.Pin = HashPin(staff.Pin);
            _repository.AddStaff(staff);
        }

        public void UpdateStaff(Staff staff)
        {
            staff.Pin = HashPin(staff.Pin);
            _repository.UpdateStaff(staff);
        }

        public void UpdateStaffInfo(Staff staff)
        {
            _repository.UpdateStaff(staff);
        }

        public void DeleteStaff(int id)
        {
            _repository.DeleteStaff(id);
        }

        public bool VerifyStaffPin(Staff staff, string pin)
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
