namespace DashboardManagement.Model
{
    public class DashboardResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long TotalUsers { get; set; }
        public long ActiveMembers { get; set; }
        public long TotalMemories { get; set; }
        public long PeriodMemories { get; set; }
        public long OriginUsers { get; set; }
        public long HeartUsers { get; set; }
        public long FamilyUsers { get; set; }
        public double MembershipRevenue { get; set; }
        public double GiftRevenue { get; set; }
        public double TotalRevenue => MembershipRevenue + GiftRevenue;
        public long TotalGifts { get; set; }
        public long GiftVoucherUsers { get; set; }
        public long RegularUsers { get; set; }
        public long ExpiredTrialUsers { get; set; }
        public long ExpiredPackageUsers { get; set; }
        public long TotalLikes { get; set; }
        public long TotalComments { get; set; }
        public long TotalCandles { get; set; }
        public double AverageInteractionsPerMemory { get; set; }
        public long ReportedContentCount { get; set; }
        public List<DashboardTrendPoint> Trend { get; set; } = new();
        public List<DashboardRecentActivity> RecentActivities { get; set; } = new();
    }

    public class DashboardTrendPoint
    {
        public DateTime Date { get; set; }
        public long NewUsers { get; set; }
        public double MembershipRevenue { get; set; }
        public double GiftRevenue { get; set; }
    }

    public class DashboardRecentActivity
    {
        public string Type { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ActorName { get; set; }
        public double? Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class UserStats
    {
        public long TotalUsers { get; set; }
        public long ActiveMembers { get; set; }
        public long OriginUsers { get; set; }
        public long HeartUsers { get; set; }
        public long FamilyUsers { get; set; }
        public double MembershipRevenue { get; set; }
        public double GiftRevenue { get; set; }
        public long TotalGifts { get; set; }
        public long GiftVoucherUsers { get; set; }
        public long RegularUsers { get; set; }
        public long ExpiredTrialUsers { get; set; }
        public long ExpiredPackageUsers { get; set; }
        public List<DashboardTrendPoint> Trend { get; set; } = new();
        public List<DashboardRecentActivity> RecentActivities { get; set; } = new();
    }

    public class MemoryStats
    {
        public long TotalMemories { get; set; }
        public long PeriodMemories { get; set; }
        public long TotalLikes { get; set; }
        public long TotalComments { get; set; }
        public long TotalCandles { get; set; }
        public double AverageInteractionsPerMemory { get; set; }
        public List<DashboardRecentActivity> RecentActivities { get; set; } = new();
    }
}
