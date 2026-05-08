using System.Net;

namespace Booking.Exceptions
{
    public abstract class TermsAndConditionsException : AppException
    {
        protected TermsAndConditionsException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class TermsNotFoundException : TermsAndConditionsException
    {
        public TermsNotFoundException(int id)
            : base($"Terms & Conditions with id '{id}' was not found.", (int)HttpStatusCode.NotFound) { }
    }
}
