using Booking.Strategies;
using Booking.Constants;

namespace Booking.Factories
{
    public class ProfileStrategyFactory : IProfileStrategyFactory
    {
        private readonly Dictionary<string, Func<IProfileStrategy>> _strategies;

        public ProfileStrategyFactory(
            BasicProfileStrategy basic,
            AgencyOwnerProfileStrategy agencyOwner)
        {
            _strategies = new Dictionary<string, Func<IProfileStrategy>>
            {
                [Roles.SuperAdmin] = () => basic,
                [Roles.Customer] = () => basic,
                [Roles.AgencyOwner] = () => agencyOwner,
            };
        }

        public IProfileStrategy Create(string role)
        {
            if (!_strategies.TryGetValue(role, out var factory))
                throw new InvalidOperationException($"No strategy found for role: {role}");

            return factory();
        }
    }
}
