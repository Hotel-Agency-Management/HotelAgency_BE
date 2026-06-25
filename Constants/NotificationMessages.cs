namespace Booking.Constants
{
    public static class NotificationTitles
    {
        // Reservation
        public const string ReservationConfirmed = "Reservation Confirmed";
        public const string NewReservation = "New Reservation";
        public const string NewOnlineReservation = "New Online Reservation";
        public const string CheckOutProcessed = "Check-Out Processed";
        public const string ReservationCancelled = "Reservation Cancelled";
        public const string ReservationCancelledByGuest = "Reservation Cancelled by Guest";
        public const string ReservationUpdatedByGuest = "Reservation Updated by Guest";
        public const string RefundIssued = "Refund Issued";

        // Ticket
        public const string NewTicketAssigned = "New Ticket Assigned";
        public const string TicketAssigned = "Ticket Assigned";
        public const string TicketStatusUpdated = "Ticket Status Updated";

        // Payment
        public const string PaymentReceived = "Payment Received";

        // System
        public const string EmailVerified = "Email Verified";
        public const string PasswordChanged = "Password Changed";
        public const string PasswordReset = "Password Reset";

        // Admin
        public const string NewAgencyRegistered = "New Agency Registered";
        public const string NewHotelCreated = "New Hotel Created";
    }

    public static class NotificationMessages
    {
        // Reservation — {0} = ReservationNumber, {1} = GuestFullName or agency name
        public const string ReservationConfirmed = "Your reservation #{0} has been confirmed.";
        public const string NewReservation = "Reservation #{0} has been created for {1}.";
        public const string NewOnlineReservation = "New online reservation #{0} received from {1}.";
        public const string CheckOutProcessed = "Your check-out for reservation #{0} has been processed. Thank you for your stay!";
        public const string ReservationCancelled = "Your reservation #{0} has been cancelled.";
        public const string ReservationCancelledByGuest = "Reservation #{0} has been cancelled by the guest.";
        public const string ReservationUpdatedByGuest = "Reservation #{0} has been updated by the guest.";
        public const string RefundIssued = "A refund of {0:F2} has been issued for reservation #{1}.";

        // Ticket — {0} = ticket title, {1} = new status
        public const string TicketAssigned = "You have been assigned to ticket '{0}'.";
        public const string TicketStatusUpdated = "Ticket '{0}' status has been changed to {1}.";

        // Payment — {0} = amount
        public const string PaymentReceived = "A payment of {0:F2} has been credited to your account.";

        // System
        public const string EmailVerified = "Your email has been verified. Welcome! Your account is now active.";
        public const string PasswordChanged = "Your password has been changed successfully. If you did not make this change, please contact support immediately.";
        public const string PasswordReset = "Your password has been reset successfully. If you did not request this, please contact support immediately.";

        // Admin — {0} = agency/hotel name, {1} = agency name
        public const string NewAgencyRegistered = "A new agency '{0}' has been registered.";
        public const string NewHotelCreated = "A new hotel '{0}' has been created under agency '{1}'.";
    }
}
