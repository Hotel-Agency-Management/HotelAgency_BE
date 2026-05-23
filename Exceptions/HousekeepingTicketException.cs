using System.Net;

namespace Booking.Exceptions
{
    public abstract class HousekeepingTicketException : AppException
    {
        protected HousekeepingTicketException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class HousekeepingTicketNotFoundException : HousekeepingTicketException
    {
        public HousekeepingTicketNotFoundException(int ticketId)
            : base($"Housekeeping ticket with id '{ticketId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class HousekeepingTicketValidationException : HousekeepingTicketException
    {
        public HousekeepingTicketValidationException(string detail)
            : base(detail, (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
