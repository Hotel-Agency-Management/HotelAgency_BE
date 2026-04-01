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
        Task<bool> ValidateResetCodeAsync(string email, string code);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
    }
}
