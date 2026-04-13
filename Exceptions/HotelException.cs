using System.Net;

namespace Booking.Exceptions
{
    public abstract class HotelException : AppException
    {
        protected HotelException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class HotelNotFoundException : HotelException
    {
        public HotelNotFoundException(int hotelId)
            : base($"Hotel with id '{hotelId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class HotelValidationException : HotelException
    {
        public HotelValidationException()
            : base("Invalid Hotel Data", (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
