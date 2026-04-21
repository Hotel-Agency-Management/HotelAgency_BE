using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface IEmailVerificationService
    {
        Task<string> GenerateVerificationLinkAsync(ApplicationUser user);
    }
}
