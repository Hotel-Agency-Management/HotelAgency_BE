using Booking.DTO;
using Booking.Models;
using Booking.Strategies;

namespace Booking.Factories
{
    public interface IProfileStrategyFactory
    {
        Task<BaseProfileResponseDto> BuildProfileAsync(string role, ApplicationUser user);
    }
}
