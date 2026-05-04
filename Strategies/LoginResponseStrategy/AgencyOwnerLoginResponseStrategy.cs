using Booking.DTO;
using Booking.Enums;
using Booking.Models;

namespace Booking.Strategies
{
    public class AgencyOwnerLoginResponseStrategy : ILoginResponseStrategy
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
                AgencyStatus = agencyStatus,
                AgencyId = agency?.Id,
                HotelId = null,
                AgencyTheme = agency == null ? null : new ThemeDto
                {
                    PrimaryColor = agency.PrimaryColor,
                    SecondaryColor = agency.SecondaryColor,
                    TertiaryColor = agency.TertiaryColor,
                    LogoUrl = agency.LogoUrl
                },
                HotelTheme = null
            };
        }
    }
}
