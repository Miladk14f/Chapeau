using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Chapeau.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;

        public StaffService(IStaffRepository repository)
        {
            _repository = repository;
        }

        public List<Staff> GetAllStaff() => _repository.GetAllStaff();

        public List<Staff> GetAllStaffWithoutPins()
        {
            List<Staff> staff = _repository.GetAllStaff();
            foreach (Staff s in staff)
                s.Pin = null;
            return staff;
        }

        public Staff GetStaffById(int id) => _repository.GetStaffById(id);

        public Staff TryLogin(int staffId, string password)
        {
            Staff staff = _repository.GetStaffById(staffId);
            if (staff == null || staff.Pin != HashPin(password))
                return null;
            return staff;
        }

        public void CreateStaff(string name, string role, string pin)
        {
            Staff staff = new Staff
            {
                Name = name,
                Role = Enum.Parse<StaffRole>(role, ignoreCase: true),
                Pin = HashPin(pin)
            };
            _repository.AddStaff(staff);
        }

        public bool UpdateStaffDetails(int id, string name, string role, string pin)
        {
            Staff staff = _repository.GetStaffById(id);
            if (staff == null)
                return false;

            staff.Name = name;
            staff.Role = Enum.Parse<StaffRole>(role, ignoreCase: true);

            if (!string.IsNullOrWhiteSpace(pin))
                staff.Pin = HashPin(pin);

            _repository.UpdateStaff(staff);
            return true;
        }

        public void DeleteStaff(int id) => _repository.DeleteStaff(id);

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
