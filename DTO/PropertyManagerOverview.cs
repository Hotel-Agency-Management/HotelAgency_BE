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
}
