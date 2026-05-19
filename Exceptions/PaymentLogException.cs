using System.Net;

namespace Booking.Exceptions
{
    public abstract class PaymentLogException : AppException
    {
        protected PaymentLogException(string message, int statusCode)
            : base(message, statusCode) { }
    }

    public class PaymentLogNotFoundException : PaymentLogException
    {
        public PaymentLogNotFoundException(int paymentLogId)
            : base($"Payment log with id '{paymentLogId}' was not found.", (int)HttpStatusCode.NotFound) { }
    }

    public class PaymentLogForbiddenException : PaymentLogException
    {
        public PaymentLogForbiddenException(int paymentLogId, int hotelId)
            : base($"Payment log with id '{paymentLogId}' does not belong to hotel with id '{hotelId}'.", (int)HttpStatusCode.Forbidden) { }
    }
}
