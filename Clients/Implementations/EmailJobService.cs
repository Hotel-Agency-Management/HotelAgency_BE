using Booking.Clients;
//using Booking.DTO.Email;
using Hangfire;

namespace Booking.Clients;

public class EmailJobService(
    IBackgroundJobClient _jobs,
    IEmailService _emailService) : IEmailJobService
{
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
            (current, kv) => current.Replace($"{{{{{kv.Key}}}}}", kv.Value)
        );
    }

}
