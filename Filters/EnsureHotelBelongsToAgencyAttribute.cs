using Booking.Constants;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureHotelBelongsToAgencyAttribute : TypeFilterAttribute
    {
        public EnsureHotelBelongsToAgencyAttribute() : base(typeof(EnsureHotelBelongsToAgencyFilter))
        {
        }
    }

    public class EnsureHotelBelongsToAgencyFilter(
        UserManager<ApplicationUser> _userManager,
        IHotelRepository _hotelRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("hotelId", out var hotelIdValue) ||
                hotelIdValue is not int hotelId)
            {
                context.Result = new BadRequestObjectResult(Messages.HotelIdMissing);
                return;
            }

            var agencyOwner = await _userManager.GetUserAsync(context.HttpContext.User);
            if (agencyOwner is null)
            {
                context.Result = new UnauthorizedObjectResult(Messages.Unauthorized);
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
                context.Result = new NotFoundObjectResult(new
                {
                    statusCode = 404,
                    message = string.Format(Messages.HotelNotFound, hotelId)
                });
                return;
            }

            if (hotel.AgencyId != agencyOwner.AgencyId.Value)
            {
                context.Result = new ObjectResult(new
                {
                    statusCode = 403,
                    message = Messages.HotelForbidden
                })
                { StatusCode = 403 };
                return;
            }

            await next();
        }
    }
}
