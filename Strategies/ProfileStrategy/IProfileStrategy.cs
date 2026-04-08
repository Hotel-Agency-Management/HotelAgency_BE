using Booking.DTO;
using Booking.Models;

namespace Booking.Strategies
{
    public interface IProfileStrategy
    {
        Task<BaseProfileResponseDto> BuildProfileAsync(ApplicationUser user);
    }
}
