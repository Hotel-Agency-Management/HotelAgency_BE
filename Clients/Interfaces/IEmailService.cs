namespace Booking.Clients;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string plainText, string htmlContent);
    Task<string> LoadTemplateAsync(string templateName);
    string RenderTemplate(string template, Dictionary<string, string> placeholders);
}
