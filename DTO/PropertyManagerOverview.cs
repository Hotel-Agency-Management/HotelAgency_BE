namespace Booking.DTO
{
    public class PropertyManagerOverviewCardsResponse
    {
        public int TotalReservations { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public int PendingReservations { get; set; }
    }
}
