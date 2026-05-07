using Booking.Constants;
using Booking.Strategies;

namespace Booking.Factories
{
    public class LoginResponseStrategyFactory(
        AgencyOwnerLoginResponseStrategy agencyOwner,
        HotelStaffLoginResponseStrategy hotelStaff,
        DefaultLoginResponseStrategy defaultStrategy) : ILoginResponseStrategyFactory
    {
        private static readonly HashSet<string> HotelStaffRoles =
        [
            Roles.PropertyManager,
            Roles.FrontDeskStaff,
            Roles.HousekeepingManager,
            Roles.HousekeepingEmployee,
            Roles.Accountant,
            Roles.CustomerSupport,
            Roles.Auditor,
        ];

        public ILoginResponseStrategy GetStrategy(string role) => role switch
        {
            Roles.AgencyOwner => agencyOwner,
            _ when HotelStaffRoles.Contains(role) => hotelStaff,
            _ => defaultStrategy
        };
    }
}
