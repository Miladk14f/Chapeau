namespace Chapeau.ViewModels
{
    public class PreparationCard
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public string StaffName { get; set; }
        public DateTime OrderedAt { get; set; }
        public List<PreparationItemRow> Items { get; set; } = new();

        public bool IsPreparing { get; set; }

        public int WarningMinutes { get; set; }
        public int UrgentMinutes { get; set; }

        public int MinutesAgo => (int)(DateTime.Now - OrderedAt).TotalMinutes;
        public bool IsUrgent => MinutesAgo >= UrgentMinutes;
        public bool IsWarning => MinutesAgo >= WarningMinutes && MinutesAgo < UrgentMinutes;
    }

    public class PreparationItemRow
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }

        public int MinutesAgo => (int)(DateTime.Now - CreatedAt).TotalMinutes;
    }
}
