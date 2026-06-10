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
}
