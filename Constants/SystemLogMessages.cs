namespace Booking.Constants
{
    public static class SystemLogMessages
    {
        // Plan — {0} = plan name
        public const string PlanCreated = "Plan '{0}' was created.";
        public const string PlanUpdated = "Plan '{0}' was updated.";

        // Agency — {0} = agency name
        public const string AgencyUpdated = "Agency '{0}' details were updated.";

        // Hotel — {0} = hotel name
        public const string HotelCreated = "Hotel '{0}' was created.";
        public const string HotelUpdated = "Hotel '{0}' was updated.";

        // Room — {0} = room number
        public const string RoomCreated = "Room '{0}' was created.";
        public const string RoomUpdated = "Room '{0}' was updated.";

        // Reservation — {0} = reservation number, {1} = guest name / new status
        public const string ReservationCreated = "Reservation '{0}' was created for {1}.";
        public const string ReservationCheckedIn = "Reservation '{0}' status changed to CheckedIn.";
        public const string ReservationCheckedOut = "Reservation '{0}' status changed to CheckedOut.";
        public const string ReservationCancelled = "Reservation '{0}' was cancelled.";

        // User — {0} = email, {1} = role (where applicable)
        public const string UserRegistered = "New user '{0}' registered with role {1}.";
        public const string UserLogin = "User '{0}' logged in.";
        public const string UserPasswordReset = "Password was reset for user '{0}'.";
        public const string UserEmailVerified = "Email verified for user '{0}'.";
    }
}
