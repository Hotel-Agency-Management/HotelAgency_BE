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

    public class ManagerDoesNotBelongToAgencyException : HotelException
    {
        public ManagerDoesNotBelongToAgencyException(int managerUserId, int agencyId)
            : base($"Manager user with id '{managerUserId}' does not belong to agency with id '{agencyId}'.", (int)HttpStatusCode.BadRequest)
        {
        }
    }

    public class ManagerMustBePropertyManagerException : HotelException
    {
        public ManagerMustBePropertyManagerException(int managerUserId)
            : base($"Manager user with id '{managerUserId}' must have the PROPERTY_MANAGER role.", (int)HttpStatusCode.BadRequest)
        {
        }
    }

    public class ManagerAlreadyAssignedToHotelException : HotelException
    {
        public ManagerAlreadyAssignedToHotelException(int managerUserId, int hotelId)
            : base($"Manager user with id '{managerUserId}' is already assigned to hotel with id '{hotelId}'.", (int)HttpStatusCode.Conflict)
        {
        }
    }

    public class HotelNotAssignedException : AppException
    {
        public HotelNotAssignedException()
            : base("User has no Hotel assigned.", StatusCodes.Status400BadRequest)
        {
        }
    }

}
