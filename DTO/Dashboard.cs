namespace Booking.DTO
{
    public class MonthlyRevenueItem
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class RevenueOverviewResponse
    {
        public IReadOnlyList<MonthlyRevenueItem> Revenue { get; set; } = [];
    }

    public class DashboardSummaryResponse
    {
        public int TotalAgencies { get; set; }
        public int PendingApprovals { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class AgencyStatusSlice
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AgencyStatusChartResponse
    {
        public int Total { get; set; }
        public IReadOnlyList<AgencyStatusSlice> Breakdown { get; set; } = [];
    }

    public class SubscriptionSlice
    {
        public string PlanName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SubscriptionDistributionResponse
    {
        public int Total { get; set; }
        public IReadOnlyList<SubscriptionSlice> Breakdown { get; set; } = [];
    }

    public class AgencyGrowthItem
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AgencyGrowthResponse
    {
        public IReadOnlyList<AgencyGrowthItem> Growth { get; set; } = [];
    }
}
