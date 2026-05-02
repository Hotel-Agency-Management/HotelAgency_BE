namespace Booking.Exceptions
{
    public class ReservationNotFoundException : AppException
    {
        public ReservationNotFoundException(int id)
            : base($"Reservation with id {id} not found.", 404) { }
    }

    public class RoomNotAvailableException : AppException
    {
        public RoomNotAvailableException()
            : base("The selected room is not available for the requested dates.", 409) { }
    }

    public class InvalidStatusTransitionException : AppException
    {
        public InvalidStatusTransitionException(string from, string to)
            : base($"Cannot transition reservation from '{from}' to '{to}'.", 400) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, 400) { }
    }
}
