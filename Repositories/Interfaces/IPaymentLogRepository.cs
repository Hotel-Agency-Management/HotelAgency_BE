using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public record HotelPaymentSummary(
        decimal TotalIncoming,
        decimal TotalOutgoing,
        int IncomingCount,
        int OutgoingCount);

    public record MonthlyRevenue(int Year, int Month, decimal Revenue);
    public record HotelRevenue(int HotelId, string HotelName, decimal Revenue);

    public interface IPaymentLogRepository
    {
        Task<PaymentLog> CreateAsync(PaymentLog paymentLog);

        // Admin — all platform logs
        Task<IEnumerable<PaymentLog>> GetAllPagedAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize);
        Task<int> CountAllAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo);

        // Hotel — paged (incoming=null=all, true=incoming only, false=outgoing only)
        Task<IEnumerable<PaymentLog>> GetHotelLogsAsync(int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize);
        Task<int> CountHotelLogsAsync(int hotelId, bool? incoming, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);

        // Hotel — unfiltered summary totals in one query
        Task<HotelPaymentSummary> GetHotelSummaryAsync(int hotelId);

        // Single record with Reservation included
        Task<PaymentLog?> GetByIdAsync(int paymentLogId);

        // Agency overview stats
        Task<decimal> GetTotalIncomingByAgencyAsync(int agencyId);
        Task<IEnumerable<MonthlyRevenue>> GetMonthlyIncomingByAgencyAsync(int agencyId, DateTime from, DateTime to);
        Task<IEnumerable<MonthlyRevenue>> GetMonthlyOutgoingByAgencyAsync(int agencyId, DateTime from, DateTime to);
        Task<IEnumerable<HotelRevenue>> GetRevenuePerHotelByAgencyAsync(int agencyId);
    }
}
