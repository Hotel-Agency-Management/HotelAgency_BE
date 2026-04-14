using System.Net;

namespace Booking.Exceptions
{
    public abstract class RoomException : AppException
    {
        protected RoomException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class RoomNotFoundException : RoomException
    {
        public RoomNotFoundException(int roomId)
            : base($"Room with id {roomId} Not Found", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class RoomAlreadyExistsException : RoomException
    {
        public RoomAlreadyExistsException()
            : base("Room Already Exists", (int)HttpStatusCode.Conflict)
        {
        }
    }

    public class RoomTypeNotInHotelException : RoomException
    {
        public RoomTypeNotInHotelException()
            : base("Invalid Room Type", (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
