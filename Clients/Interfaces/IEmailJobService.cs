using Booking.Models;
namespace Booking.Clients;

public interface IEmailJobService
{
    Task EnqueueVerificationEmailAsync(ApplicationUser user, string verificationLink);
    Task EnqueueAgencyUnderReviewEmailAsync(ApplicationUser user);
    Task EnqueueTeamMemberVerificationEmailAsync(ApplicationUser user, Agency agency, string verificationLink, string password);
}
