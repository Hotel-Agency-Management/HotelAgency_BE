using Booking.DTO;
using Booking.Models;

namespace Booking.Strategies
{
    public class BasicProfileStrategy : IProfileStrategy
    {
        public Task<BaseProfileResponseDto> BuildProfileAsync(ApplicationUser user)
            => Task.FromResult(new BaseProfileResponseDto(user));
    }
}
