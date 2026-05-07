using Booking.Enums;

namespace Booking.Interfaces.Services
{
    public interface ICustomerAccountService
    {
        Task<int?> EnsureCustomerAsync(
            ReservationSource source,
            int? customerId,
            string guestEmail,
            string guestFullName,
            string guestPhone);
    }
}
