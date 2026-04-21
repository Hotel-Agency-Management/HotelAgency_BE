using Booking.Interfaces.Repositories;
using Booking.Utils;
using Booking.Models;
using Booking.Interfaces.Services;

namespace Booking.Services
{
    public class EmailVerificationService(
        IAuthRepository _authRepository) : IEmailVerificationService
    {
        public async Task<string> GenerateVerificationLinkAsync(ApplicationUser user)
        {
            await _authRepository.DeleteExistingEmailVerificationTokensAsync(user.Id);

            var rawToken = AuthUtils.GenerateSecureToken();
            var hashedToken = AuthUtils.HashToken(rawToken);

            await _authRepository.SaveEmailVerificationTokenAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            });

            return $"http://localhost:3000/verify-email?userId={user.Id}&token={Uri.EscapeDataString(rawToken)}";
        }
    }
}