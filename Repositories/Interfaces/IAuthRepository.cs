using Booking.Models;
using Microsoft.AspNetCore.Identity;


namespace Booking.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByIdAsync(int id);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<string> GetRoleAsync(ApplicationUser user);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task DeleteExistingCodesAsync(int userId);
        Task SaveResetCodeAsync(PasswordResetCode resetCode);
        Task<PasswordResetCode?> GetValidResetCodeAsync(int userId, string code);
        Task MarkCodeAsUsedAsync(PasswordResetCode resetCode);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword);
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetValidRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
        Task DeleteUserRefreshTokensAsync(int userId);
        Task SaveEmailVerificationTokenAsync(EmailVerificationToken token);
        Task DeleteExistingEmailVerificationTokensAsync(int userId);
        Task<EmailVerificationToken?> GetValidEmailVerificationTokenAsync(int userId, string tokenHash);
        Task MarkEmailVerificationTokenAsUsedAsync(EmailVerificationToken token);
        Task<bool> ConfirmEmailAsync(ApplicationUser user);

    }
}
