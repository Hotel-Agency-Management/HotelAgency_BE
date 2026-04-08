using Booking.Enums;
using Booking.Strategies;

namespace Booking.Factories
{
    public interface IProfileStrategyFactory
    {
        IProfileStrategy Create(string role);
    }
}
