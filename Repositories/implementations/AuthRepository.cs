using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Booking.Data;
using Microsoft.EntityFrameworkCore;
using Booking.Exceptions;

namespace Booking.Repositories
{
    public class AuthRepository(
        UserManager<ApplicationUser> _userManager,
        ApplicationDbContext _context) : IAuthRepository
    {
        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<string> GetRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? string.Empty;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
                await _userManager.UpdateSecurityStampAsync(user);

            return result;
        }

        public async Task DeleteExistingCodesAsync(int userId)
        {
            var existingCodes = _context.PasswordResetCodes
                .Where(x => x.UserId == userId && !x.IsUsed);
            _context.PasswordResetCodes.RemoveRange(existingCodes);
            await _context.SaveChangesAsync();
        }

        public async Task SaveResetCodeAsync(PasswordResetCode resetCode)
        {
            await _context.PasswordResetCodes.AddAsync(resetCode);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetCode?> GetValidResetCodeAsync(int userId, string code)
        {
            return await _context.PasswordResetCodes
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Code == code &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task MarkCodeAsUsedAsync(PasswordResetCode resetCode)
        {
            resetCode.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == token &&
                    !x.IsRevoked &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserRefreshTokensAsync(int userId)
        {
            var tokens = _context.RefreshTokens
                .Where(x => x.UserId == userId);
            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationUser?> FindByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }
        public async Task SaveEmailVerificationTokenAsync(EmailVerificationToken token)
        {
            _context.EmailVerificationTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExistingEmailVerificationTokensAsync(int userId)
        {
            var tokens = _context.EmailVerificationTokens
                .Where(x => x.UserId == userId && !x.IsUsed);

            _context.EmailVerificationTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailVerificationToken?> GetValidEmailVerificationTokenAsync(int userId, string tokenHash)
        {
            return await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.TokenHash == tokenHash &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task MarkEmailVerificationTokenAsUsedAsync(EmailVerificationToken token)
        {
            token.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmEmailAsync(ApplicationUser user)
        {
            user.EmailConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<ApplicationUser> GetUserWithAgencyAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Agency)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new UserNotFoundException($"User with id {userId} not found.");

        }

        public async Task<ApplicationUser> GetUserWithAgencyAndHotelAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.Hotel)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new UserNotFoundException($"User with id {userId} not found.");
        }
    }
}
