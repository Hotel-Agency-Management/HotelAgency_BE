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

    // Housekeeping Tickets
    public const string TicketIdMissing = "TicketId is missing.";
    public const string TicketNotFound = "Ticket with id '{0}' was not found.";
    public const string TicketForbidden = "Ticket with id '{0}' does not belong to hotel with id '{1}'.";

    // Ticket Comments
    public const string CommentIdMissing = "CommentId is missing.";
    public const string CommentNotFound = "Ticket comment with id '{0}' was not found.";
    public const string CommentForbidden = "Comment with id '{0}' does not belong to ticket with id '{1}'.";

    //reservation
    public const string ContractFileMustBePdf = "Contract file must be a PDF.";
    public const string InvoiceFileMustBePdf = "Invoice file must be a PDF.";
    public const string InvalidCheckOutDate = "Check-out date must be after check-in date.";
    public const string ReservationAlreadyCancelled = "Reservation is already cancelled.";
    public const string ReservationNotUpdatable = "Reservation cannot be updated in its current status.";
    public const string ReservationCannotBeCancelledDueToStatus = "Reservation cannot be cancelled in its current status.";
    public const string FreeCancellationMessage = "Reservation cancelled successfully with no cancellation fee.";
    public const string PaidCancellationMessage = "Reservation cancelled. A cancellation fee has been applied.";
    public const string CheckInDateInThePast = "Check-in date cannot be in the past.";
    public const string RoomsNotAvailableStatus = "The following rooms are not available for booking due to their current status: {0}.";
    public const string ReservationCannotBeCancelledAfterCheckIn =
        "Reservation cannot be cancelled on or after the check-in date.";
    public const string ReservationCannotBeCancelledAfterCheckOut =
        "Reservation cannot be cancelled after the check-out date.";
    public const string InvalidReservationStatusTransition =
        "Status can only be changed between CheckedIn and CheckedOut.";

}
