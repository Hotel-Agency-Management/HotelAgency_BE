using Booking.Models;
namespace Booking.Clients;

public interface IEmailJobService
{
    Task EnqueueVerificationEmailAsync(ApplicationUser user, string verificationLink);
    Task EnqueueAgencyUnderReviewEmailAsync(ApplicationUser user);
    Task EnqueueTeamMemberVerificationEmailAsync(ApplicationUser user, Agency agency, string verificationLink, string password);
    Task EnqueueReservationConfirmationEmailAsync(string recipientEmail, string guestName, Reservation reservation, string contractUrl, string invoiceUrl);
    Task EnqueueNewCustomerAccountEmailAsync(ApplicationUser user, string verificationLink, string defaultPassword);
    Task EnqueueTeamMemberProfileUpdatedEmailAsync(ApplicationUser user, Agency agency, Hotel? hotel);
}
