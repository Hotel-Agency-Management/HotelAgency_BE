using Booking.Constants;
using Booking.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Booking.Filters
{
    public class EnsureHotelExistsForAdminAttribute : TypeFilterAttribute
    {
        public EnsureHotelExistsForAdminAttribute() : base(typeof(EnsureHotelExistsForAdminFilter))
        {
        }
    }

    public class EnsureHotelExistsForAdminFilter(
        IAgencyRepository _agencyRepository,
        IHotelRepository _hotelRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetId(context, "agencyId", Messages.AgencyIdMissing, out int agencyId))
                return;

            if (!FilterHelpers.TryGetId(context, "hotelId", Messages.HotelIdMissing, out int hotelId))
                return;

            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency is null)
            {
                context.Result = FilterHelpers.NotFound($"Agency with id '{agencyId}' was not found.");
                return;
            }

            var hotel = await _hotelRepository.GetByIdAsync(hotelId);
            if (hotel is null)
            {
                context.Result = FilterHelpers.NotFound(string.Format(Messages.HotelNotFound, hotelId));
                return;
            }

            if (hotel.AgencyId != agencyId)
            {
                context.Result = FilterHelpers.Forbidden($"Hotel with id '{hotelId}' does not belong to agency with id '{agencyId}'.");
                return;
            }

            await next();
        }
    }
}