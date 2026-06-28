using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class TableOverviewViewModel
    {
        public List<TableCard> Cards { get; set; } = new();
    }

    public class TableCard
    {
        public RestaurantTable Table { get; set; }
        public List<ReadyItemRow> ReadyFood { get; set; } = new();
        public List<ReadyItemRow> ReadyDrink { get; set; } = new();
        public List<ReadyItemRow> PreparingFood { get; set; } = new();
        public List<ReadyItemRow> PreparingDrink { get; set; } = new();

        public int ReadyFoodCount => CountItems(ReadyFood);
        public int ReadyDrinkCount => CountItems(ReadyDrink);
        public int PreparingFoodCount => CountItems(PreparingFood);
        public int PreparingDrinkCount => CountItems(PreparingDrink);

        private static int CountItems(List<ReadyItemRow> rows)
        {
            int count = 0;
            foreach (ReadyItemRow row in rows)
                count += row.Qty;
            return count;
        }
    }

    public class ReadyItemRow
    {
        public string Name { get; set; }
        public int Qty { get; set; }
    }
}
