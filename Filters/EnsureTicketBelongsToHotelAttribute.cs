using Booking.Constants;
using Booking.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureTicketBelongsToHotelAttribute : TypeFilterAttribute
    {
        public EnsureTicketBelongsToHotelAttribute() : base(typeof(EnsureTicketBelongsToHotelFilter))
        {
        }
    }

    public class EnsureTicketBelongsToHotelFilter(
        IHousekeepingTicketRepository _ticketRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetRouteId(context, "hotelId", Messages.HotelIdMissing, out int hotelId))
                return;

            if (!FilterHelpers.TryGetRouteId(context, "ticketId", Messages.TicketIdMissing, out int ticketId))
                return;

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket is null)
            {
                context.Result = FilterHelpers.NotFound(string.Format(Messages.TicketNotFound, ticketId));
                return;
            }

            if (ticket.HotelId != hotelId)
            {
                context.Result = FilterHelpers.Forbidden(string.Format(Messages.TicketForbidden, ticketId, hotelId));
                return;
            }

            await next();
        }
    }
}
