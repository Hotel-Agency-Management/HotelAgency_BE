using System.Net;
using Booking.Constants;

namespace Booking.Exceptions
{
    public abstract class RoomTypeException : AppException
    {
        protected RoomTypeException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class RoomTypeNotFoundException : RoomTypeException
    {
        public RoomTypeNotFoundException(int roomTypeId)
            : base($"room type with id '{roomTypeId}' was not found.", (int)HttpStatusCode.NotFound) { }
    }

    public class RoomTypeAlreadyExistsException : RoomTypeException
    {
        public RoomTypeAlreadyExistsException()
            : base($"Room Type AlreadyExists", (int)HttpStatusCode.Conflict) { }
    }
}
