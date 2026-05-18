namespace Booking.Constants
{
    public static class Roles
    {
        public const string SuperAdmin = "SUPER_ADMIN";
        public const string AgencyOwner = "AGENCY_OWNER";
        public const string PropertyManager = "PROPERTY_MANAGER";
        public const string FrontDeskStaff = "FRONT_DESK_STAFF";
        public const string HousekeepingManager = "HOUSEKEEPING_MANAGER";
        public const string HousekeepingEmployee = "HOUSEKEEPING_EMPLOYEE";
        public const string Accountant = "ACCOUNTANT";
        public const string CustomerSupport = "CUSTOMER_SUPPORT";
        public const string Auditor = "AUDITOR";
        public const string Customer = "CUSTOMER";
        public static readonly string[] All =
        {
            SuperAdmin,
            AgencyOwner,
            PropertyManager,
            FrontDeskStaff,
            HousekeepingManager,
            HousekeepingEmployee,
            Accountant,
            CustomerSupport,
            Auditor,
            Customer
        };
    }
}
