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

}
