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

    public Task EnqueueTeamMemberInviteEmailAsync(ApplicationUser user, string verificationLink, string password)
    {
        var userName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(userName))
            userName = "User";

        var plainText =
            $"Hi {userName}, you have been invited to HotelAgency. " +
            $"Verify your email: {verificationLink}. " +
            $"Email: {user.Email}. Temporary password: {password}";

        var html = $"""
            <p>Hi {userName},</p>
            <p>You have been invited to HotelAgency.</p>
            <p><a href="{verificationLink}">Verify your email</a></p>
            <p>Email: <strong>{user.Email}</strong></p>
            <p>Temporary password: <strong>{password}</strong></p>
            <p>Please change your password after signing in.</p>
            """;

        _jobs.Enqueue<IEmailService>(svc =>
            svc.SendEmailAsync(user.Email!, "You have been invited to HotelAgency", plainText, html));

        return Task.CompletedTask;
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
