namespace Booking.Constants;

public static class PaymentReason
{
    public const string Booking              = "Payment for room booking";
    public const string Cancellation         = "Payment associated with reservation cancellation";
    public const string ReservationInsurance = "Insurance fee applied per reservation";
    public const string YearlyInsurance      = "Annual insurance premium";
    public const string Extend               = "Payment for reservation extension";
    public const string Damage               = "Charge for property damage";
    public const string Refund               = "Refund issued to customer";
}
