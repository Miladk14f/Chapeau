using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public StaffRole Role { get; set; }
        public string Pin { get; set; }

        public Staff() { }

        public Staff(int staffId, string name, StaffRole role, string pin)
        {
            StaffId = staffId;
            Name = name;
            Role = role;
            Pin = pin;
        }
    }
}
