using System.Net;

namespace Booking.Exceptions
{
    public abstract class NotificationException : AppException
    {
        protected NotificationException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class NotificationNotFoundException : NotificationException
    {
        public NotificationNotFoundException(int id)
            : base($"Notification with id '{id}' was not found.", (int)HttpStatusCode.NotFound) { }
    }

    public class NotificationAccessDeniedException : NotificationException
    {
        public NotificationAccessDeniedException(int id)
            : base($"You do not have access to notification '{id}'.", (int)HttpStatusCode.Forbidden) { }
    }
}
