using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IPaymentLogRepository
    {
        Task<PaymentLog> CreateAsync(PaymentLog paymentLog);

        // Admin — all platform logs
        Task<IEnumerable<PaymentLog>> GetAllPagedAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize);
        Task<int> CountAllAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo);

        // Hotel — paged (incoming=null=all, true=incoming only, false=outgoing only)
        Task<IEnumerable<PaymentLog>> GetHotelLogsAsync(int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize);
        Task<int> CountHotelLogsAsync(int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);

        // Hotel — unfiltered summary totals
        Task<decimal> SumHotelIncomingAsync(int hotelId);
        Task<decimal> SumHotelOutgoingAsync(int hotelId);
        Task<int> CountHotelIncomingAsync(int hotelId);
        Task<int> CountHotelOutgoingAsync(int hotelId);

        // Single record with Reservation included
        Task<PaymentLog?> GetByIdAsync(int paymentLogId);
    }
}
