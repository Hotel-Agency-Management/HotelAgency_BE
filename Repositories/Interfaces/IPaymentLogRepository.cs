using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IPaymentLogRepository
    {
        Task<PaymentLog> CreateAsync(PaymentLog paymentLog);
    }
}
