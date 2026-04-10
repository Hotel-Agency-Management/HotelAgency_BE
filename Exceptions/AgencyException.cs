using System.Net;

namespace Booking.Exceptions
{
    public class AgencyException : AppException
    {
        public AgencyException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class AgencyNotFoundException : AgencyException
    {
        public AgencyNotFoundException(int agencyId)
            : base($"Agency with id '{agencyId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class AgencyUnderReviewException : AgencyException
    {
        public AgencyUnderReviewException()
            : base($"Your agency account is still under review. Please wait for approval before logging in.", (int)HttpStatusCode.Forbidden)
        {
        }
    }

    public class AgencyAlreadyExistsException : AgencyException
    {
        public AgencyAlreadyExistsException(string agencyName)
            : base($"Agency '{agencyName}' already exists.", (int)HttpStatusCode.Conflict)
        {
        }
    }

    public class AgencyOwnerNotFoundException : AgencyException
    {
        public AgencyOwnerNotFoundException(int ownerId)
            : base($"Owner with id '{ownerId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class AgencyAlreadyDeactivatedException : AgencyException
    {
        public AgencyAlreadyDeactivatedException(int agencyId)
            : base($"Agency with id '{agencyId}' is already deactivated.", (int)HttpStatusCode.BadRequest)
        {
        }
    }

    public class InvalidAgencyDocumentException : AgencyException
    {
        public InvalidAgencyDocumentException()
            : base("Invalid document uploaded.", (int)HttpStatusCode.BadRequest)
        {
        }
    }

    public class AgencyNotAssignedException : AppException
    {
        public AgencyNotAssignedException()
            : base("User has no agency assigned.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
