using System.Net;

namespace Booking.Exceptions
{
    public class PlanException : AppException
    {
        public PlanException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class PlanNotFoundException : PlanException
    {
        public PlanNotFoundException(int planId)
            : base($"Plan with id '{planId}' was not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }

    public class PlanAlreadyExistsException : PlanException
    {
        public PlanAlreadyExistsException(string planName)
            : base($"Plan '{planName}' already exists.", (int)HttpStatusCode.Conflict)
        {
        }
    }

    public class PlanAlreadyActiveException : PlanException
    {
        public PlanAlreadyActiveException(int planId)
            : base($"Plan with id '{planId}' is already active.", (int)HttpStatusCode.BadRequest)
        {
        }
    }

    public class PlanAlreadyInactiveException : PlanException
    {
        public PlanAlreadyInactiveException(int planId)
            : base($"Plan with id '{planId}' is already inactive.", (int)HttpStatusCode.BadRequest)
        {
        }
    }

    public class PlanNotAvailableException : PlanException
    {
        public PlanNotAvailableException(int planId)
            : base($"Plan with id '{planId}' is not available for selection.", (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
