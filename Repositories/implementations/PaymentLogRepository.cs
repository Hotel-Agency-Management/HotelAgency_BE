using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;

namespace Booking.Repositories
{
    public class PaymentLogRepository(ApplicationDbContext _context) : IPaymentLogRepository
    {
        public async Task<PaymentLog> CreateAsync(PaymentLog paymentLog)
        {
            await _context.PaymentLogs.AddAsync(paymentLog);
            await _context.SaveChangesAsync();
            return paymentLog;
        }
    }
}
