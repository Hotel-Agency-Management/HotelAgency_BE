using Booking.Interfaces.Repositories;
using Booking.Utils;
using Booking.Models;
using Booking.Interfaces.Services;

namespace Booking.Services
{
    public class EmailVerificationService(
        IAuthRepository _authRepository,
        IAppLinkService _appLinkService,
        ILogger<EmailVerificationService> _logger) : IEmailVerificationService
    {
        public async Task<string> GenerateVerificationLinkAsync(ApplicationUser user)
        {
            _logger.LogDebug("Generating email verification link for user {UserId}", user.Id);
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

            return _appLinkService.BuildVerifyEmailLink(user.Id, rawToken);
        }
    }
}
