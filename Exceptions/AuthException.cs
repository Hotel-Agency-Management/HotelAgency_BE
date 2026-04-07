using System.Net;
namespace Booking.Exceptions
{

    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.", (int)HttpStatusCode.Unauthorized) { }
    }


    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string email)
            : base($"User with email '{email}' was not found.", (int)HttpStatusCode.NotFound) { }
    }


    public class EmailAlreadyExistsException : AppException
    {
        public EmailAlreadyExistsException(string email)
            : base($"Email '{email}' is already registered.", (int)HttpStatusCode.Conflict) { }
    }

    public class RegistrationFailedException : AppException
    {
        public RegistrationFailedException(string message)
            : base(message, (int)HttpStatusCode.BadRequest) { }

    }

    public class InvalidRefreshTokenException : AppException
    {
        public InvalidRefreshTokenException()
            : base("Invalid or expired refresh token.", (int)HttpStatusCode.Unauthorized) { }
    }

    public class InvalidResetCodeException : AppException
    {
        public InvalidResetCodeException()
            : base("Invalid or expired reset code", (int)HttpStatusCode.BadRequest) { }
    }

    public class EmailNotConfirmedException : AppException
    {
        public EmailNotConfirmedException(string email)
            : base($"Email '{email}' is not confirmed.", (int)HttpStatusCode.Forbidden) { }
    }

}
