using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureAgencyExistsForAdminAttribute : TypeFilterAttribute
    {
        public EnsureAgencyExistsForAdminAttribute() : base(typeof(EnsureAgencyExistsForAdminFilter))
        {
        }
    }

    public class EnsureAgencyExistsForAdminFilter(
        IAgencyRepository _agencyRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetRouteId(context, "agencyId", "AgencyId is missing.", out int agencyId))
                return;

            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency is null)
                throw new AgencyNotFoundException(agencyId);

            await next();
        }
    }
}
