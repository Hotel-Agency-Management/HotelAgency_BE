namespace Booking.Constants
{
    public static class EmailTemplates
    {
        public const string ResetPassword =
            "Hi {0}, use the following code to reset your password: {1}. " +
            "This code expires in 15 minutes. If you didn’t request this, you can safely ignore this email.";

        public const string VerifyEmail =
            "Verify your email using this link: {0}";
    }
}
