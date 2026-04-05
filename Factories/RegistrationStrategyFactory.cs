using Booking.Enums;
using Booking.Strategies;

namespace Booking.Factories
{
    public class RegistrationStrategyFactory : IRegistrationStrategyFactory
    {
        private readonly Dictionary<AccountType, IRegistrationStrategy> _strategyMap;

        public RegistrationStrategyFactory(
            CustomerRegistrationStrategy customerStrategy,
            AgencyOwnerRegistrationStrategy agencyOwnerStrategy)
        {
            _strategyMap = new()
        {
            { AccountType.Customer,    customerStrategy    },
            { AccountType.AgencyOwner, agencyOwnerStrategy },
        };
        }

        public IRegistrationStrategy GetStrategy(AccountType accountType)
        {
            if (!_strategyMap.TryGetValue(accountType, out var strategy))
                throw new InvalidOperationException($"Unsupported account type: {accountType}");

            return strategy;
        }
    }
}