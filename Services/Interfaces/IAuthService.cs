using Booking.DTO.Auth;
using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<ApplicationUser> RegisterAsync(RegisterRequest request);

    }
}
