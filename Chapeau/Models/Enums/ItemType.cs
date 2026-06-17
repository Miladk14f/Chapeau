namespace Chapeau.Models.Enums
{
    public enum ItemType
    {
        Starters,
        Mains,
        Desserts,
        Entremets,
        SoftDrinks,
        CoffeeTea,
        Beer,
        Wine,
        Spirits
    }

    public static class ItemTypeGroups
    {
        public static readonly ItemType[] Food = { ItemType.Starters, ItemType.Mains, ItemType.Desserts, ItemType.Entremets };
        public static readonly ItemType[] Drinks = { ItemType.SoftDrinks, ItemType.CoffeeTea, ItemType.Beer, ItemType.Wine, ItemType.Spirits };
    }
}
