using Booking.DTO;
using Booking.Models;

namespace Booking.Strategies
{
    public static class BaseProfileStrategy
    {
        public static Task<BaseProfileResponseDto> BuildProfileAsync(ApplicationUser user)
            => Task.FromResult(new BaseProfileResponseDto(user));
    }
}
