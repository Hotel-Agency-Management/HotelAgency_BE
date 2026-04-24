using Booking.Strategies;
using Booking.Constants;
using Booking.DTO;
using Booking.Models;

namespace Booking.Factories
{
    public class ProfileStrategyFactory : IProfileStrategyFactory
    {
        private readonly Dictionary<string, Func<ApplicationUser, Task<BaseProfileResponseDto>>> _strategies;

        public ProfileStrategyFactory(
            AgencyOwnerProfileStrategy agencyOwner,
            HotelStaffProfileStrategy hotelStaff)
        {
            _strategies = new Dictionary<string, Func<ApplicationUser, Task<BaseProfileResponseDto>>>
            {
                [Roles.SuperAdmin] = BaseProfileStrategy.BuildProfileAsync,
                [Roles.Customer] = BaseProfileStrategy.BuildProfileAsync,
                [Roles.AgencyOwner] = agencyOwner.BuildProfileAsync,
                [Roles.PropertyManager] = hotelStaff.BuildProfileAsync,
                [Roles.FrontDeskStaff] = hotelStaff.BuildProfileAsync,
                [Roles.HousekeepingManager] = hotelStaff.BuildProfileAsync,
                [Roles.HousekeepingEmployee] = hotelStaff.BuildProfileAsync,
                [Roles.Accountant] = hotelStaff.BuildProfileAsync,
                [Roles.CustomerSupport] = hotelStaff.BuildProfileAsync,
                [Roles.Auditor] = hotelStaff.BuildProfileAsync,
            };
        }

        public Task<BaseProfileResponseDto> BuildProfileAsync(string role, ApplicationUser user)
        {
            if (!_strategies.TryGetValue(role, out var buildProfile))
                throw new InvalidOperationException($"No strategy found for role: {role}");

            return buildProfile(user);
        }
    }
}
