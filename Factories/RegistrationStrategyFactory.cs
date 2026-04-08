using Booking.Enums;
using Booking.Strategies;

namespace Booking.Factories
{
    public class RegistrationStrategyFactory(
        CustomerRegistrationStrategy _customerStrategy,
        AgencyOwnerRegistrationStrategy _agencyOwnerStrategy
    ) : IRegistrationStrategyFactory
    {
        private Dictionary<AccountType, IRegistrationStrategy> BuildMap() => new()
        {
            { AccountType.Customer,    _customerStrategy    },
            { AccountType.AgencyOwner, _agencyOwnerStrategy },
        };

        public IRegistrationStrategy GetStrategy(AccountType accountType)
        {
            var map = BuildMap();

            if (!map.TryGetValue(accountType, out var strategy))
                throw new InvalidOperationException($"Unsupported account type: {accountType}");

            return strategy;
        }
    }
}
