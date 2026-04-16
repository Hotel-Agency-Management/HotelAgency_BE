using System.Net;

namespace Booking.Exceptions
{
    public abstract class FacilityException : AppException
    {
        protected FacilityException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class FacilityNotFoundException : FacilityException
    {
        public FacilityNotFoundException(int facilityId)
            : base($"Facility with id '{facilityId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class FacilityAlreadyExistsException : FacilityException
    {
        public FacilityAlreadyExistsException()
            : base("Facility with this name already exists for the given hotel.", (int)HttpStatusCode.Conflict)
        {
        }
    }
}
