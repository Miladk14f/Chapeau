using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Category { get; set; }
        public SubCategory SubCategory { get; set; }

        public decimal Price { get; set; }
        public int Vat { get; set; }
        public string Allergens { get; set; }
        public bool InStock { get; set; }

        public string CategoryDisplayName => Category.ToString(); // Lunch / Dinner / Drinks

        public string SubCategoryDisplayName => SubCategory switch
        {
            SubCategory.SoftDrinks => "Soft Drinks",
            SubCategory.CoffeeTea => "Coffee/Tea",
            _ => SubCategory.ToString()
        };

        public MenuItem() { }

        public MenuItem(int menuItemId, string name, string description, string category, string subCategory, decimal price, int vat, string allergens, bool inStock)
        {
            MenuItemId = menuItemId;
            Name = name;
            Description = description;
            Category = MapCategory(category);
            SubCategory = MapSubCategory(subCategory);
            Price = price;
            Vat = vat;
            Allergens = allergens;
            InStock = inStock;
        }

    private static ItemType MapCategory(string category) =>
        (category ?? "").Trim().ToLower() switch
        {
            "lunch" => ItemType.Lunch,
            "dinner" => ItemType.Dinner,
            "drinks" => ItemType.Drinks,
            _ => throw new ArgumentException($"Unknown category: {category}")
        };

    private static SubCategory MapSubCategory(string subCategory) =>
        (subCategory ?? "").Trim().ToLower() switch
        {
            "starters" => SubCategory.Starters,
            "mains" => SubCategory.Mains,
            "desserts" => SubCategory.Desserts,
            "entremets" => SubCategory.Entremets,
            "soft drinks" => SubCategory.SoftDrinks,
            "coffee/tea" => SubCategory.CoffeeTea,
            "beer" => SubCategory.Beer,
            "wine" => SubCategory.Wine,
            "spirits" => SubCategory.Spirits,
            _ => throw new ArgumentException($"Unknown subcategory: {subCategory}")
        };
    }
}
