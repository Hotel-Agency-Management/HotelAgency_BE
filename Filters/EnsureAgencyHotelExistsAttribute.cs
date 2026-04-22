using Booking.Constants;
using Booking.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureAgencyHotelExistsAttribute : TypeFilterAttribute
    {
        public EnsureAgencyHotelExistsAttribute() : base(typeof(EnsureAgencyHotelExistsFilter))
        {
        }
    }

    public class EnsureAgencyHotelExistsFilter(
        IAgencyRepository _agencyRepository,
        IHotelRepository _hotelRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("agencyId", out var agencyIdValue) ||
                agencyIdValue is not int agencyId)
            {
                context.Result = new BadRequestObjectResult(Messages.AgencyIdMissing);
                return;
            }

            if (!context.ActionArguments.TryGetValue("hotelId", out var hotelIdValue) ||
                hotelIdValue is not int hotelId)
            {
                context.Result = new BadRequestObjectResult(Messages.HotelIdMissing);
                return;
            }

            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency is null)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    statusCode = 404,
                    message = $"Agency with id '{agencyId}' was not found."
                });
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

            if (hotel.AgencyId != agencyId)
            {
                context.Result = new ObjectResult(new
                {
                    statusCode = 403,
                    message = $"Hotel with id '{hotelId}' does not belong to agency with id '{agencyId}'."
                })
                { StatusCode = 403 };
                return;
            }

            await next();
        }
    }
}
