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
}
