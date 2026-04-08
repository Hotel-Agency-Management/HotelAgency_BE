using Booking.Enums;
using Booking.Strategies;

namespace Booking.Factories
{
    public interface IRegistrationStrategyFactory
    {
        IRegistrationStrategy GetStrategy(AccountType accountType);
    }
}
