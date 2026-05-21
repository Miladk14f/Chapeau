namespace Chapeau.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Pin { get; set; }

        public Staff() { }

        public Staff(int id, string name, string role, string pin)
        {
            Id = id;
            Name = name;
            Role = role;
            Pin = pin;
        }
    }
}
