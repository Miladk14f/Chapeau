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

        public int ReadyFoodCount
        {
            get
            {
                int count = 0;
                foreach (ReadyItemRow row in ReadyFood)
                    count += row.Qty;
                return count;
            }
        }

        public int ReadyDrinkCount
        {
            get
            {
                int count = 0;
                foreach (ReadyItemRow row in ReadyDrink)
                    count += row.Qty;
                return count;
            }
        }
    }

    public class ReadyItemRow
    {
        public string Name { get; set; }
        public int Qty { get; set; }
    }
}
