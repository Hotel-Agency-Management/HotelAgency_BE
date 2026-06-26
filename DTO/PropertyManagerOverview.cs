namespace Booking.DTO
{
    public class PropertyManagerOverviewCardsResponse
    {
        public int TotalReservations { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public int PendingReservations { get; set; }
    }

    public class OccupancyRateItem
    {
        public string Label { get; set; } = string.Empty;
        public int OccupiedRooms { get; set; }
        public decimal OccupancyRate { get; set; }
    }

    public class OccupancyRateResponse
    {
        public int HotelId { get; set; }
        public int TotalRooms { get; set; }
        public List<OccupancyRateItem> Items { get; set; } = new();
    }

    public class HotelReservationStatusDistributionResponse
    {
        public int TotalReservations { get; set; }
        public List<ReservationStatusDistributionItem> Items { get; set; } = new();
    }

    public class HotelBookingTypeDistributionResponse
    {
        public int TotalReservations { get; set; }
        public List<BookingTypeDistributionItem> Items { get; set; } = new();
    }

    public class RevenueTrendItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class HotelRevenueTrendResponse
    {
        public string GroupBy { get; set; } = string.Empty;
        public List<RevenueTrendItem> Items { get; set; } = new();
    }

    public class InsuranceIncomePerBookingTrendItem
    {
        public string Month { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class TicketCompletionRateResponse
    {
        public decimal Value { get; set; }
    }
}
