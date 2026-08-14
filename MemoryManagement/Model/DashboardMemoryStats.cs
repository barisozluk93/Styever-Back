namespace MemoryManagement.Model
{
    public class DashboardMemoryStats
    {
        public long TotalMemories { get; set; }
        public long PeriodMemories { get; set; }
        public long TotalLikes { get; set; }
        public long TotalComments { get; set; }
        public long TotalCandles { get; set; }
        public double AverageInteractionsPerMemory { get; set; }
        public List<DashboardRecentActivity> RecentActivities { get; set; } = new();
    }

    public class DashboardRecentActivity
    {
        public string Type { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ActorName { get; set; }
        public double? Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
