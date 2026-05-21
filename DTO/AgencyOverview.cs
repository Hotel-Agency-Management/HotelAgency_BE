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
}
