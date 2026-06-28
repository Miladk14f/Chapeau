namespace Chapeau.Models.Enums
{
    public enum SubCategory
    {
        Starters,
        Mains,
        Desserts,
        Entremets,
        SoftDrinks,
        CoffeeTea,
        Beer,
        Wine,
        Spirits,
    }

    public static class ItemTypeGroups
    {
        public static readonly SubCategory[] Food = { SubCategory.Starters, SubCategory.Mains, SubCategory.Desserts, SubCategory.Entremets };
        public static readonly SubCategory[] Drinks = { SubCategory.SoftDrinks, SubCategory.CoffeeTea, SubCategory.Beer, SubCategory.Wine, SubCategory.Spirits };
    }
}
