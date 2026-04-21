using Booking.Constants;
using Hangfire;
using Booking.Models;

namespace Booking.Clients;

//TODO: support FE Base url in the appsettings.
public class EmailJobService(
    IBackgroundJobClient _jobs,
    IEmailService _emailService) : IEmailJobService
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
                { "HELP_LINK", "http://localhost:3000/help" },
                { "SUPPORT_LINK", "http://localhost:3000/support" },
                { "PRIVACY_LINK", "http://localhost:3000/privacy" },
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
            { "HELP_LINK", "http://localhost:3000/help" },
            { "SUPPORT_LINK", "http://localhost:3000/support" },
            { "PRIVACY_LINK", "http://localhost:3000/privacy" },
            }
        );
    }

    public async Task EnqueueTeamMemberVerificationEmailAsync(ApplicationUser user, Hotel hotel, string verificationLink, string password)
    {
        var userName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(userName))
            userName = "User";

        var plainText =
            $"Hi {userName}, you have been invited to {hotel.Name}. " +
            $"Verify your email: {verificationLink}. " +
            $"Email: {user.Email}. Temporary password: {password}";

        var template = await _emailService.LoadTemplateAsync(EmailTemplateFiles.HotelInvitation);
        var html = RenderTemplate(
            template,
            new Dictionary<string, string>
            {
                { "AGENCY_NAME", hotel.Name },
                { "HOTEL_NAME", hotel.Name },
                { "USER_NAME", userName },
                { "USER_EMAIL", user.Email ?? string.Empty },
                { "TEMP_PASSWORD", password },
                { "PRIMARY_COLOR", GetThemeColor(hotel.PrimaryColor, "#173f3a") },
                { "SECONDARY_COLOR", GetThemeColor(hotel.SecondaryColor, "#d8b879") },
                { "TERTIARY_COLOR", GetThemeColor(hotel.TertiaryColor, "#f8f5ef") },
                { "VERIFY_LINK", verificationLink },
                { "HELP_LINK", "http://localhost:3000/help" },
                { "SUPPORT_LINK", "http://localhost:3000/support" },
                { "PRIVACY_LINK", "http://localhost:3000/privacy" }
            });

        _jobs.Enqueue<IEmailService>(svc =>
            svc.SendEmailAsync(user.Email!, $"You have been invited to {hotel.Name}", plainText, html));
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

    private static string GetThemeColor(string color, string fallback)
    {
        return string.IsNullOrWhiteSpace(color) ? fallback : color.Trim();
    }
}
