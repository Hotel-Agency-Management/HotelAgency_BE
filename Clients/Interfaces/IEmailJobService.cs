using Booking.Models;
namespace Booking.Clients;

public interface IEmailJobService
{
    Task EnqueueVerificationEmailAsync(ApplicationUser user, string verificationLink);

}
