namespace Chapeau.ViewModels
{
    public class PreparationPageViewModel
    {
        public List<PreparationCard> Cards { get; set; } = new();
        public string PageClass { get; set; }
        public string HeaderClass { get; set; }
        public string IconClass { get; set; }
        public string IconEmoji { get; set; }
        public string SubClass { get; set; }
        public string Title { get; set; }
        public string CountLabel { get; set; }
        public string PrepLabel { get; set; }
        public string EmptyText { get; set; }
        public bool ShowTypeBadge { get; set; }
        public bool ShowZeroUrgent { get; set; }
        public string LegendNormal { get; set; }
        public string LegendWarning { get; set; }
        public string LegendUrgent { get; set; }
    }

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
