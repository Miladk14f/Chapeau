namespace Chapeau.ViewModels
{
    public class BarOrderCard
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public string StaffName { get; set; }
        public DateTime OrderedAt { get; set; }
        public List<KitchenItemRow> Items { get; set; } = new();

        public int MinutesAgo => (int)(DateTime.Now - OrderedAt).TotalMinutes;
        public bool IsUrgent => MinutesAgo >= 10;
        public bool IsWarning => MinutesAgo >= 6 && MinutesAgo < 10;
    }

    public class KitchenOrderCard
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public string StaffName { get; set; }
        public DateTime OrderedAt { get; set; }
        public List<KitchenItemRow> Items { get; set; } = new();

        public int MinutesAgo => (int)(DateTime.Now - OrderedAt).TotalMinutes;
        public bool IsUrgent => MinutesAgo >= 20;
        public bool IsWarning => MinutesAgo >= 12 && MinutesAgo < 20;
    }

    public class KitchenItemRow
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public DateTime CreatedAt { get; set; }

        public int MinutesAgo => (int)(DateTime.Now - CreatedAt).TotalMinutes;
    }
}
