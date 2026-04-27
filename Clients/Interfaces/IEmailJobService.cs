using Booking.Models;
namespace Booking.Clients;

public interface IEmailJobService
{
    Task EnqueueVerificationEmailAsync(ApplicationUser user, string verificationLink);
    Task EnqueueAgencyUnderReviewEmailAsync(ApplicationUser user);
    Task EnqueueTeamMemberVerificationEmailAsync(ApplicationUser user, Hotel hotel, string verificationLink, string password);
}
