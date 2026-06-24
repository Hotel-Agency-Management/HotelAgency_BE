using Booking.Constants;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureAgencyExistsForOwnerAttribute : TypeFilterAttribute
    {
        public EnsureAgencyExistsForOwnerAttribute() : base(typeof(EnsureAgencyExistsForOwnerFilter))
        {
        }
    }

    public class EnsureAgencyExistsForOwnerFilter(
        UserManager<ApplicationUser> _userManager,
        IAgencyRepository _agencyRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var agencyOwner = await _userManager.GetUserAsync(context.HttpContext.User);
            if (agencyOwner is null)
                throw new AuthenticatedUserNotFoundException();

            if (await _userManager.IsInRoleAsync(agencyOwner, Constants.Roles.Customer))
            {
                await next();
                return;
            }

            if (agencyOwner.AgencyId is null)
                throw new AgencyNotAssignedException();

            var agencyId = agencyOwner.AgencyId.Value;
            if (context.ActionArguments.TryGetValue("agencyId", out var routeValue))
            {
                if (routeValue is not int routeAgencyId)
                    throw new AgencyIdMissingException();

                if (routeAgencyId != agencyId)
                    throw new AgencyAccessDeniedException(routeAgencyId);
            }

            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency is null)
                throw new AgencyNotFoundException(agencyId);

            context.HttpContext.Items[FilterHelpers.AgencyIdItemKey] = agencyId;
            await next();
        }
    }
}
