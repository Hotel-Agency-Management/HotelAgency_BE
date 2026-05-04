using Booking.Strategies;

namespace Booking.Factories
{
    public interface ILoginResponseStrategyFactory
    {
        ILoginResponseStrategy GetStrategy(string role);
    }
}
