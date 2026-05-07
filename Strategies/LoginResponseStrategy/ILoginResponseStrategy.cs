using Booking.DTO;
using Booking.Enums;
using Booking.Models;

namespace Booking.Strategies
{
    public interface ILoginResponseStrategy
    {
        AuthResponseDto BuildResponse(
            ApplicationUser user,
            string role,
            string token,
            string refreshToken,
            Agency? agency,
            AgencyStatus? agencyStatus,
            Hotel? hotel);
    }
}
