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
            templateFile: "verify-email.html",
            to: user.Email!,
            subject: "Verify Your Email",
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
            templateFile: "agency-under-review.html",
            to: user.Email!,
            subject: "Your Agency is Under Review",
            plainText: $"Hi {userName}, your agency is currently under review. You will be notified once it has been approved or rejected.",
            placeholders: new Dictionary<string, string>
            {
            { "AGENCY_NAME", userName },
            { "HELP_LINK", "http://localhost:3000/help" },
            { "SUPPORT_LINK", "http://localhost:3000/support" },
            { "PRIVACY_LINK", "http://localhost:3000/privacy" },
            }
        );
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
}
