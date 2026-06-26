namespace Booking.Constants
{
    public static class DashboardConstants
    {
        public const int LatestAgenciesCount = 10;
        public const int RecentLogsCount = 4;

        public const string GroupByDaily   = "daily";
        public const string GroupByMonthly = "monthly";

        public const int DailyTrendDaysBack    = -29;
        public const int DailyTrendDaysForward = 1;
        public const int DailyTrendDaysCount   = 30;

        public const int AgencyTrendMonthsBack = -12;

        public const string DailyLabelFormat = "dd MMM";
        public const string IsoDateFormat    = "yyyy-MM-dd";
        public const string MonthKeyFormat   = "yyyy-MM";

        public const int PercentageMultiplier = 100;
        public const int DecimalPlaces        = 2;
    }

    public static class BookingSourceLabels
    {
        public const string Online = "Online";
        public const string OTA    = "OTA";
        public const string Phone  = "Phone";
        public const string WalkIn = "Walk-in";
    }
}
