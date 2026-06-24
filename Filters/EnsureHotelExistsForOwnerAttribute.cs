using Booking.Constants;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureHotelExistsForOwnerAttribute : TypeFilterAttribute
    {
        public EnsureHotelExistsForOwnerAttribute() : base(typeof(EnsureHotelExistsForOwnerFilter))
        {
        }
    }

    public class EnsureHotelExistsForOwnerFilter(
        UserManager<ApplicationUser> _userManager,
        IHotelRepository _hotelRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetId(context, "hotelId", Messages.HotelIdMissing, out int hotelId))
                return;

            var agencyOwner = await _userManager.GetUserAsync(context.HttpContext.User);
            if (agencyOwner is null)
            {
                context.Result = new UnauthorizedObjectResult(Messages.Unauthorized);
                return;
            }

            if (await _userManager.IsInRoleAsync(agencyOwner, Roles.Customer))
            {
                await next();
                return;
            }

            if (agencyOwner.AgencyId is null)
            {
                context.Result = new BadRequestObjectResult(Messages.AgencyIdMissing);
                return;
            }

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel is null)
            {
                context.Result = FilterHelpers.NotFound(string.Format(Messages.HotelNotFound, hotelId));
                return;
            }

            if (hotel.AgencyId != agencyOwner.AgencyId.Value)
            {
                context.Result = FilterHelpers.Forbidden(Messages.HotelForbidden);
                return;
            }

            await next();
        }
    }
}
