using Booking.Constants;
using Booking.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Filters
{
    public class EnsureCommentBelongsToTicketAttribute : TypeFilterAttribute
    {
        public EnsureCommentBelongsToTicketAttribute() : base(typeof(EnsureCommentBelongsToTicketFilter))
        {
        }
    }

    public class EnsureCommentBelongsToTicketFilter(
        ITicketCommentRepository _commentRepository) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!FilterHelpers.TryGetRouteId(context, "ticketId", Messages.TicketIdMissing, out int ticketId))
                return;

            if (!FilterHelpers.TryGetRouteId(context, "commentId", Messages.CommentIdMissing, out int commentId))
                return;

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment is null)
            {
                context.Result = FilterHelpers.NotFound(string.Format(Messages.CommentNotFound, commentId));
                return;
            }

            if (comment.TicketId != ticketId)
            {
                context.Result = FilterHelpers.Forbidden(string.Format(Messages.CommentForbidden, commentId, ticketId));
                return;
            }

            await next();
        }
    }
}
