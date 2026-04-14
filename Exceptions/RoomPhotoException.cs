using System.Net;
using Booking.Constants;

namespace Booking.Exceptions
{
    public class RoomPhotoNotFoundException : AppException
    {
        public RoomPhotoNotFoundException(int photoId)
            : base($"Photo with id {photoId} Not Found", (int)HttpStatusCode.NotFound) { }
    }
}
