public static class Messages
{
    // Auth 
    public const string Unauthorized = "Unauthorized";
    public const string RoleNotFound = "Role claim not found.";
    public const string UserAccountIsArchived = "Account is not Archived.";

    // Password
    public const string PasswordChangeFailed = "Password change failed";
    public const string PasswordChangedSuccessfully = "Password changed successfully";

    // Forgot Password
    public const string NoAccountWithEmail = "No account associated with this email";
    public const string FailedToSendResetEmail = "Failed to send reset email, please try again";
    public const string PasswordResetEmailSent = "Password reset email sent";

    // Reset Code
    public const string InvalidOrExpiredCode = "Invalid or expired reset code";
    public const string CodeIsValid = "Code is valid";

    // Reset Password
    public const string PasswordResetSuccessfully = "Password reset successfully";

    // Email
    public const string EmailVerifiedSuccessfully = "Email verified successfully";
    public const string VerificationEmailSent = "Verification email sent";

    // Register
    public const string AgencyUnderReview =
        "Your agency is under review. You will receive an email once it has been approved. Please also check your inbox to verify your email address.";

    public const string VerifyEmail =
        "Please check your email to verify your account";

    // Agency
    public const string LogoUpdatedSuccessfully = "Logo Updated Successfully";
    public const string AgencyUpdatedSuccessfully = "Agency Updated Successfully";

    //filter
    public const string HotelIdMissing = "HotelId is missing.";
    public const string AgencyIdMissing = "AgencyId is missing.";
    public const string FacilityIdMissing = "FacilityId is missing.";
    public const string HotelNotFound = "Hotel with id '{0}' was not found.";
    public const string HotelForbidden = "You do not have permission to access this hotel.";
    public const string FacilityNotFound = "Facility with id '{0}' was not found.";
    public const string FacilityForbidden = "Facility with id '{0}' does not belong to hotel with id '{1}'.";

    //reservation 
    public const string ContractFileMustBePdf = "Contract file must be a PDF.";
    public const string InvoiceFileMustBePdf = "Invoice file must be a PDF.";
    public const string InvalidCheckOutDate = "Check-out date must be after check-in date.";

}
