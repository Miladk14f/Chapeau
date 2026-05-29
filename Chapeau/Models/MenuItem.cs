namespace Chapeau.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Vat { get; set; }
        public string Allergens { get; set; }
        public bool InStock { get; set; }

        public MenuItem() { }

        public MenuItem(int menuItemId, string name, string description, string category, decimal price, int vat, string allergens, bool inStock)
        {
            MenuItemId = menuItemId;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            Vat = vat;
            Allergens = allergens;
            InStock = inStock;
        }
    }
}
