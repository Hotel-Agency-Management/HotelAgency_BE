using Booking.DTO;
using Booking.Enums;
using Booking.Models;

namespace Booking.Strategies
{
    public class DefaultLoginResponseStrategy : ILoginResponseStrategy
    {
        public AuthResponseDto BuildResponse(
            ApplicationUser user,
            string role,
            string token,
            string refreshToken,
            Agency? agency,
            AgencyStatus? agencyStatus,
            Hotel? hotel)
        {
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Role = role,
                AgencyTheme = null,
                HotelTheme = null
            };
        }
    }
}
