using System.Net;

namespace Booking.Exceptions
{
    public abstract class TicketCommentException : AppException
    {
        protected TicketCommentException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class TicketCommentNotFoundException : TicketCommentException
    {
        public TicketCommentNotFoundException(int commentId)
            : base($"Ticket comment with id '{commentId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class TicketCommentForbiddenException : TicketCommentException
    {
        public TicketCommentForbiddenException()
            : base("You do not have permission to perform this action on this comment.", (int)HttpStatusCode.Forbidden)
        {
        }
    }

    public class TicketCommentValidationException : TicketCommentException
    {
        public TicketCommentValidationException(string detail)
            : base(detail, (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
