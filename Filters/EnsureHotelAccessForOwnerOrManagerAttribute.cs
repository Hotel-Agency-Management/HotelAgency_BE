using Booking.Constants;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureHotelAccessForOwnerOrManagerAttribute : TypeFilterAttribute
    {
        public EnsureHotelAccessForOwnerOrManagerAttribute() : base(typeof(EnsureHotelAccessForOwnerOrManagerFilter))
        {
        }
    }

    public class EnsureHotelAccessForOwnerOrManagerFilter(
        UserManager<ApplicationUser> _userManager,
        IHotelRepository _hotelRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetId(context, "hotelId", Messages.HotelIdMissing, out int hotelId))
                return;

            var user = await _userManager.GetUserAsync(context.HttpContext.User);
            if (user is null)
            {
                context.Result = FilterHelpers.Forbidden(Messages.Unauthorized);
                return;
            }

            if (user.AgencyId.HasValue)
            {
                var hotel = await _hotelRepository.GetByIdAsync(hotelId);
                if (hotel is null)
                {
                    context.Result = FilterHelpers.NotFound(string.Format(Messages.HotelNotFound, hotelId));
                    return;
                }

                if (hotel.AgencyId != user.AgencyId.Value)
                {
                    context.Result = FilterHelpers.Forbidden(Messages.HotelForbidden);
                    return;
                }
            }
            else if (user.HotelId.HasValue)
            {
                if (user.HotelId.Value != hotelId)
                {
                    context.Result = FilterHelpers.Forbidden(Messages.HotelForbidden);
                    return;
                }
            }
            else
            {
                context.Result = FilterHelpers.Forbidden(Messages.HotelForbidden);
                return;
            }

            await next();
        }
    }
}
