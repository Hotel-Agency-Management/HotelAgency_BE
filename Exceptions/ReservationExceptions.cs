namespace Booking.Exceptions
{
    public class ReservationNotFoundException : AppException
    {
        public ReservationNotFoundException(int id)
            : base($"Reservation with id {id} not found.", 404) { }
    }

    public class RoomsNotAvailableException : AppException
    {
        public RoomsNotAvailableException(IEnumerable<string> roomNumbers)
            : base($"The following rooms are not available for the requested dates: {string.Join(", ", roomNumbers)}.", 409) { }
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
