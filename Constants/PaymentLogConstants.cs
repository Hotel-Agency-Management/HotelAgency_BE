namespace Booking.Constants
{
    public static class PaymentTimelineEvents
    {
        public const string ReservationCreated = "Reservation Created";
        public const string PaymentRecorded    = "Payment Recorded";
        public const string CheckedIn          = "Checked In";
        public const string CheckedOut         = "Checked Out";
        public const string Cancelled          = "Cancelled";
    }

    public static class TransactionTypes
    {
        public const string Incoming = "Incoming";
        public const string Outgoing = "Outgoing";
    }

    public static class PaymentLogConstants
    {
        public const string FallbackHotelName = "Hotel";
    }
}
