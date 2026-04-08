using System.Net;
namespace Booking.Exceptions
{
     public class AuthException : AppException
    {
        public AuthException(string message, int statusCode)
            : base(message, statusCode)
        {
        }
    }

    public class InvalidCredentialsException : AuthException
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.", (int)HttpStatusCode.Unauthorized) { }
    }


    public class UserNotFoundException : AuthException
    {
        public UserNotFoundException(string email)
            : base($"User with email '{email}' was not found.", (int)HttpStatusCode.NotFound) { }
    }


    public class EmailAlreadyExistsException : AuthException
    {
        public EmailAlreadyExistsException(string email)
            : base($"Email '{email}' is already registered.", (int)HttpStatusCode.Conflict) { }
    }

    public class RegistrationFailedException : AuthException
    {
        public RegistrationFailedException(string message)
            : base(message, (int)HttpStatusCode.BadRequest) { }

    }

    public class InvalidRefreshTokenException : AuthException
    {
        public InvalidRefreshTokenException()
            : base("Invalid or expired refresh token.", (int)HttpStatusCode.Unauthorized) { }
    }

    public class InvalidResetCodeException : AuthException
    {
        public InvalidResetCodeException()
            : base("Invalid or expired reset code", (int)HttpStatusCode.BadRequest) { }
    }

    public class EmailNotConfirmedException : AuthException
    {
        public EmailNotConfirmedException(string email)
            : base($"Email '{email}' is not confirmed.", (int)HttpStatusCode.Forbidden) { }
    }

}
