using Booking.DTO;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Booking.Exceptions;

namespace Booking.Strategies
{
    public class HotelStaffProfileStrategy(IAuthRepository _authRepository) : IProfileStrategy
    {
        public async Task<BaseProfileResponseDto> BuildProfileAsync(ApplicationUser user)
        {
            var result = await _authRepository.GetUserWithAgencyAndHotelAsync(user.Id);

            if (result.Agency == null)
                throw new AgencyNotAssignedException();

            if (result.Hotel == null)
                throw new HotelNotAssignedException();

            return new HotelStaffProfileResponseDto(result);
        }

    }

}
