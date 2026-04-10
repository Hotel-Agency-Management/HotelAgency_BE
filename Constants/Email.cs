namespace Booking.Constants
{
    public static class EmailTemplateFiles
    {
        public const string VerifyEmail = "verify-email.html";
        public const string AgencyUnderReview = "agency-under-review.html";
    }


    public static class EmailSubjects
    {
        public const string VerifyEmail = "Verify your email";
        public const string AgencyUnderReview = "Your agency is under review";
    }

    public static class EmailTemplates
    {
        public const string ResetPassword =
            "Hi {0}, use the following code to reset your password: {1}. " +
            "This code expires in 15 minutes. If you didn’t request this, you can safely ignore this email.";

        public const string VerifyEmail =
            "Verify your email using this link: {0}";

        public const string AgencyUnderReview =
           "Hi {0}, your agency is currently under review. You will be notified once it has been approved or rejected.";
    }
}
