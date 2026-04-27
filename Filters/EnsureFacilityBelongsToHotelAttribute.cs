using Booking.Constants;
using Booking.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Filters
{
    public class EnsureFacilityBelongsToHotelAttribute : TypeFilterAttribute
    {
        public EnsureFacilityBelongsToHotelAttribute() : base(typeof(EnsureFacilityBelongsToHotelFilter))
        {
        }
    }

    public class EnsureFacilityBelongsToHotelFilter(
        IFacilityRepository _facilityRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetRouteId(context, "hotelId", Messages.HotelIdMissing, out int hotelId))
                return;

            if (!FilterHelpers.TryGetRouteId(context, "facilityId", Messages.FacilityIdMissing, out int facilityId))
                return;

            var facility = await _facilityRepository.GetByIdAsync(facilityId);
            if (facility is null)
            {
                context.Result = FilterHelpers.NotFound(string.Format(Messages.FacilityNotFound, facilityId));
                return;
            }

            if (facility.HotelId != hotelId)
            {
                context.Result = FilterHelpers.Forbidden(string.Format(Messages.FacilityForbidden, facilityId, hotelId));
                return;
            }

            await next();
        }
    }
}
