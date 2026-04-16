using System.Net;

namespace Booking.Exceptions
{
    public class FacilityPhotoNotFoundException : AppException
    {
        public FacilityPhotoNotFoundException(int photoId)
            : base($"Facility Photo with {photoId} NotFound", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class RoomPhotoNotFoundException : AppException
    {
        public RoomPhotoNotFoundException(int photoId)
            : base($"Room Photo with {photoId} NotFound", (int)HttpStatusCode.NotFound)
        {
        }
    }
}
