using System.Net;
using Booking.Constants;

namespace Booking.Exceptions
{
    public abstract class RoomAmenityException : AppException
    {
        protected RoomAmenityException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class RoomAmenityNotFoundException : RoomAmenityException
    {
        public RoomAmenityNotFoundException(int amenityId)
            : base($"Room Amenity with Id '{amenityId}' NotFound", (int)HttpStatusCode.NotFound)
        { }
    }

    public class RoomAmenityAlreadyExistsException : RoomAmenityException
    {
        public RoomAmenityAlreadyExistsException()
            : base($"Room Amenity AlreadyExists", (int)HttpStatusCode.Conflict) { }
    }
    public class SomeAmenitiesNotFoundException : RoomAmenityException
    {
        public SomeAmenitiesNotFoundException(List<int> missingIds)
            : base(
                $"Room amenities with IDs [{string.Join(", ", missingIds)}] were not found.",
                (int)HttpStatusCode.NotFound)
        {
        }
    }
}
