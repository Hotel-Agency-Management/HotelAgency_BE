namespace Booking.Exceptions
{

    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.", 401) { }
    }


    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string email)
            : base($"User with email '{email}' was not found.", 404) { }
    }


    public class EmailAlreadyExistsException : AppException
    {
        public EmailAlreadyExistsException(string email)
            : base($"Email '{email}' is already registered.", 409) { }
    }

    public class RegistrationFailedException : AppException
    {
        public RegistrationFailedException(string message)
            : base(message, 400) { }

    }

    public class InvalidRefreshTokenException : AppException
    {
        public InvalidRefreshTokenException()
            : base("Invalid or expired refresh token.", 401) { }
    }

    public class InvalidResetCodeException : AppException
    {
        public InvalidResetCodeException()
            : base("Invalid or expired reset code", 400) { }
    }

    public class EmailNotConfirmedException : AppException
    {
        public EmailNotConfirmedException(string email)
            : base($"Email '{email}' is not confirmed.", 403) { }
    }

}
