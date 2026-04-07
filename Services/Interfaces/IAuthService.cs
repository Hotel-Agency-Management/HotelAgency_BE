using Booking.DTO.Auth;
using Booking.Models;
using Microsoft.AspNetCore.Identity;


namespace Booking.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<ApplicationUser> RegisterAsync(RegisterRequest request);
        Task<ApplicationUser> UpdateProfileAsync(ApplicationUser user, UpdateProfileDto dto);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, ChangePasswordDto dto);
        Task<bool> SendResetPasswordEmailAsync(string email);
        Task ResetPasswordAsync(string email, string code, string newPassword);
        Task<bool> ValidateResetCodeAsync(string email, string code);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
        Task ResendVerificationEmailAsync(string email);
        Task VerifyEmailAsync(int userId, string token);
        Task SendVerificationEmailAsync(ApplicationUser user);

    }
}
