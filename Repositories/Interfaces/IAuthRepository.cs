using Booking.Models;
using Microsoft.AspNetCore.Identity;


namespace Booking.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
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

    }
}
