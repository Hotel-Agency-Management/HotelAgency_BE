using Booking.Constants;
using Booking.Interfaces.Services;
using Hangfire;
using Booking.Models;

namespace Booking.Clients;

public class EmailJobService(
    IBackgroundJobClient _jobs,
    IEmailService _emailService,
    IAppLinkService _appLinkService) : IEmailJobService
{
    public async Task EnqueueVerificationEmailAsync(ApplicationUser user, string verificationLink)
    {
        var userName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(userName))
            userName = "User";

        await EnqueueAsync(
            templateFile: EmailTemplateFiles.VerifyEmail,
            to: user.Email!,
            subject: EmailSubjects.VerifyEmail,
            plainText: string.Format(
                EmailTemplates.VerifyEmail,
                verificationLink
            ),
            placeholders: new Dictionary<string, string>
            {
                { "USER_NAME", userName },
                { "VERIFY_LINK", verificationLink },
                { "HELP_LINK", _appLinkService.GetHelpLink() },
                { "SUPPORT_LINK", _appLinkService.GetSupportLink() },
                { "PRIVACY_LINK", _appLinkService.GetPrivacyLink() },
                { "AGENCY_NAME", "HotelAgency" }
            }
        );
    }

    public async Task EnqueueAgencyUnderReviewEmailAsync(ApplicationUser user)
    {
        var userName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(userName))
            userName = "User";

        await EnqueueAsync(
            templateFile: EmailTemplateFiles.AgencyUnderReview,
            to: user.Email!,
            subject: EmailSubjects.AgencyUnderReview,
            plainText: string.Format(
            EmailTemplates.AgencyUnderReview,
            userName
        ),
            placeholders: new Dictionary<string, string>
            {
            { "AGENCY_NAME", userName },
            { "HELP_LINK", _appLinkService.GetHelpLink() },
            { "SUPPORT_LINK", _appLinkService.GetSupportLink() },
            { "PRIVACY_LINK", _appLinkService.GetPrivacyLink() },
            }
        );
    }

    public async Task EnqueueTeamMemberVerificationEmailAsync(ApplicationUser user, Agency agency, string verificationLink, string password)
    {
        var userName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(userName))
            userName = "User";

        var plainText =
            $"Hi {userName}, you have been invited to {agency.AgencyName}. " +
            $"Verify your email: {verificationLink}. " +
            $"Email: {user.Email}. Temporary password: {password}";

        var template = await _emailService.LoadTemplateAsync(EmailTemplateFiles.HotelInvitation);
        var html = RenderTemplate(
            template,
            new Dictionary<string, string>
            {
                { "AGENCY_NAME", agency.AgencyName },
                { "AGENCY_TAGLINE", "Management Platform" },
                { "USER_NAME", userName },
                { "USER_EMAIL", user.Email ?? string.Empty },
                { "GUEST_NAME", userName },
                { "GUEST_EMAIL", user.Email ?? string.Empty },
                { "TEMP_PASSWORD", password },
                { "PRIMARY_COLOR", GetThemeColor(agency.PrimaryColor, "#173f3a") },
                { "SECONDARY_COLOR", GetThemeColor(agency.SecondaryColor, "#d8b879") },
                { "TERTIARY_COLOR", GetThemeColor(agency.TertiaryColor, "#f8f5ef") },
                { "VERIFY_LINK", verificationLink },
                { "SUPPORT_EMAIL", "support@hotelagency.com" },
                { "HELP_LINK", _appLinkService.GetHelpLink() },
                { "SUPPORT_LINK", _appLinkService.GetSupportLink() },
                { "PRIVACY_LINK", _appLinkService.GetPrivacyLink() },
                { "CURRENT_YEAR", DateTime.UtcNow.Year.ToString() }
            });

        _jobs.Enqueue<IEmailService>(svc =>
            svc.SendEmailAsync(user.Email!, $"You have been invited to {agency.AgencyName}", plainText, html));
    }

    public async Task EnqueueReservationConfirmationEmailAsync(
        string recipientEmail, string guestName,
        Reservation reservation, string contractUrl, string invoiceUrl)
    {
        await EnqueueAsync(
            templateFile: EmailTemplateFiles.ReservationConfirmation,
            to: recipientEmail,
            subject: EmailSubjects.ReservationConfirmation,
            plainText: $"Your reservation {reservation.ReservationNumber} at {reservation.Hotel?.Name} is confirmed. Contract: {contractUrl} | Invoice: {invoiceUrl}",
            placeholders: new Dictionary<string, string>
            {
                { "HOTEL_NAME", reservation.Hotel?.Name ?? "" },
                { "GUEST_NAME", guestName },
                { "RESERVATION_NUMBER", reservation.ReservationNumber },
                { "CHECK_IN_DATE", reservation.CheckInDate.ToString("yyyy-MM-dd") },
                { "CHECK_OUT_DATE", reservation.CheckOutDate.ToString("yyyy-MM-dd") },
                { "ROOM_NUMBER", string.Join(", ", reservation.ReservationRooms.Select(rr => rr.Room?.RoomNumber ?? "")) },
                { "NUMBER_OF_GUESTS", reservation.NumberOfGuests.ToString() },
                { "NUMBER_OF_ROOMS", reservation.NumberOfRooms.ToString() },
                { "CONTRACT_LINK", contractUrl },
                { "INVOICE_LINK", invoiceUrl },
                { "HELP_LINK", _appLinkService.GetHelpLink() },
                { "SUPPORT_LINK", _appLinkService.GetSupportLink() },
                { "PRIVACY_LINK", _appLinkService.GetPrivacyLink() }
            });
    }

    public async Task EnqueueNewCustomerAccountEmailAsync(
        ApplicationUser user, string verificationLink, string defaultPassword)
    {
        var name = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(name)) name = "Guest";

        await EnqueueAsync(
            templateFile: EmailTemplateFiles.NewCustomerAccount,
            to: user.Email!,
            subject: EmailSubjects.NewCustomerAccount,
            plainText: $"Your account has been created. Email: {user.Email}, Temporary password: {defaultPassword}. Verify: {verificationLink}",
            placeholders: new Dictionary<string, string>
            {
                { "GUEST_NAME", name },
                { "EMAIL", user.Email! },
                { "DEFAULT_PASSWORD", defaultPassword },
                { "VERIFY_LINK", verificationLink },
                { "HELP_LINK", _appLinkService.GetHelpLink() },
                { "SUPPORT_LINK", _appLinkService.GetSupportLink() },
                { "PRIVACY_LINK", _appLinkService.GetPrivacyLink() }
            });
    }

    private async Task EnqueueAsync(
        string templateFile,
        string to,
        string subject,
        string plainText,
        Dictionary<string, string> placeholders)
    {
        var template = await _emailService.LoadTemplateAsync(templateFile);
        var html = RenderTemplate(template, placeholders);

        _jobs.Enqueue<IEmailService>(svc =>
            svc.SendEmailAsync(to, subject, plainText, html));
    }

    private static string RenderTemplate(string template, Dictionary<string, string> placeholders)
    {
        placeholders.TryAdd("YEAR", DateTime.UtcNow.Year.ToString());

        return placeholders.Aggregate(
            template,
            (current, kv) => current.Replace("{{" + kv.Key + "}}", kv.Value)
        );
    }

    private static string GetThemeColor(string? color, string fallback)
    {
        return string.IsNullOrWhiteSpace(color) ? fallback : color.Trim();
    }
}
