using Booking.DTO;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Booking.Exceptions;

namespace Booking.Strategies
{
    public class AgencyOwnerProfileStrategy(IAuthRepository _userRepository) : IProfileStrategy
    {
        public async Task<BaseProfileResponseDto> BuildProfileAsync(ApplicationUser user)
        {
            var result = await _userRepository.GetUserWithAgencyAndHotelAsync(user.Id);
            
            if (result.Agency == null)
                throw new AgencyNotAssignedException();

            return new AgencyOwnerProfileResponseDto(result);
        }
    }

}
