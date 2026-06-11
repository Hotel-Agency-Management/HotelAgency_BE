namespace Booking.Constants
{
    public static class SystemLogActions
    {
        public const string PlanCreated = "PLAN_CREATED";
        public const string PlanUpdated = "PLAN_UPDATED";

        public const string AgencyCreated = "AGENCY_CREATED";
        public const string AgencyUpdated = "AGENCY_UPDATED";
        public const string AgencyApproved = "AGENCY_APPROVED";
        public const string AgencyRejected = "AGENCY_REJECTED";
        public const string AgencyPlanChanged = "AGENCY_PLAN_CHANGED";

        public const string HotelCreated = "HOTEL_CREATED";
        public const string HotelUpdated = "HOTEL_UPDATED";

        public const string RoomCreated = "ROOM_CREATED";
        public const string RoomUpdated = "ROOM_UPDATED";

        public const string ReservationCreated = "RESERVATION_CREATED";
        public const string ReservationCancelled = "RESERVATION_CANCELLED";
        public const string ReservationCheckedIn = "RESERVATION_CHECKED_IN";
        public const string ReservationCheckedOut = "RESERVATION_CHECKED_OUT";

        public const string UserRegistered = "USER_REGISTERED";
        public const string UserLogin = "USER_LOGIN";
        public const string UserPasswordReset = "USER_PASSWORD_RESET";
        public const string UserEmailVerified = "USER_EMAIL_VERIFIED";
    }

    public static class SystemLogEntityTypes
    {
        public const string Plan = "Plan";
        public const string Agency = "Agency";
        public const string Hotel = "Hotel";
        public const string Room = "Room";
        public const string Reservation = "Reservation";
        public const string User = "User";
    }
}
