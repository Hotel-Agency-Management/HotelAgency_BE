using Azure;
using Azure.Communication.Email;
using Booking.Configurations;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Booking.Clients;

public class EmailService(IOptions<EmailOptions> options) : IEmailService
{
    private readonly EmailClient _emailClient = new(options.Value.ConnectionString);
    private readonly string _sender = options.Value.Sender;

    public async Task SendEmailAsync(string to, string subject, string plainText, string htmlContent)
    {
        var message = new EmailMessage(
            senderAddress: _sender,
            content: new EmailContent(subject)
            {
                PlainText = plainText,
                Html = htmlContent
            },
            recipients: new EmailRecipients([new EmailAddress(to)])
        );

        await _emailClient.SendAsync(WaitUntil.Completed, message);
    }

    public async Task<string> LoadTemplateAsync(string templateName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Booking.EmailTemplates.{templateName}";

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Email template not found: {resourceName}");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
