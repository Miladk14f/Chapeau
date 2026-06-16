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

        public List<Staff> GetAllStaff()
        {
            return _repository.GetAllStaff();
        }

        public List<Staff> GetAllStaffWithoutPins()
        {
            List<Staff> staff = _repository.GetAllStaff();
            foreach (Staff s in staff)
                s.Pin = null;
            return staff;
        }

        public Staff TryLogin(int staffId, string password)
        {
            Staff staff = _repository.GetStaffById(staffId);
            if (staff == null || !VerifyStaffPin(staff, password))
                return null;
            return staff;
        }

        public void CreateStaff(string name, string role, string pin)
        {
            Staff staff = new Staff
            {
                Name = name,
                Role = Enum.Parse<StaffRole>(role, ignoreCase: true),
                Pin = pin
            };
            AddStaff(staff);
        }

        public bool UpdateStaffDetails(int id, string name, string role, string pin)
        {
            Staff staff = _repository.GetStaffById(id);
            if (staff == null)
                return false;

            staff.Name = name;
            staff.Role = Enum.Parse<StaffRole>(role, ignoreCase: true);

            if (!string.IsNullOrWhiteSpace(pin))
            {
                staff.Pin = pin;
                UpdateStaff(staff);
            }
            else
            {
                UpdateStaffInfo(staff);
            }
            return true;
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
