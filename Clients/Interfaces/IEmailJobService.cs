using Booking.Models;
namespace Booking.Clients;

public interface IEmailJobService
{
    Task EnqueueVerificationEmailAsync(ApplicationUser user, string verificationLink);
    Task EnqueueAgencyUnderReviewEmailAsync(ApplicationUser user);
    Task EnqueueTeamMemberInviteEmailAsync(ApplicationUser user, string verificationLink, string password);
}
