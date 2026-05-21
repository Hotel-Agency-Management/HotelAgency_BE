namespace Booking.DTO
{
    public class AgencyOverviewResponse
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int PendingReservations { get; set; }
        public decimal AverageBookingValue { get; set; }
    }

    public class AgencyReservationStats
    {
        public int TotalBookings { get; set; }
        public int PendingCount { get; set; }
        public decimal AverageBookingValue { get; set; }
    }

    public class MonthlyRevenueItem
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MonthlyProfitExpensesItem
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Profit { get; set; }
        public decimal Expenses { get; set; }
    }
}
