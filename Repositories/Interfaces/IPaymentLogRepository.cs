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
    public record PaymentTypeRevenue(PaymentType Type, decimal Revenue);
    public record MonthlyNet(int Year, int Month, decimal Net);
    public record RefundImpactData(decimal PaidRevenue, decimal RefundAmount, decimal CancellationLoss);
    public record FinancialSummaryData(decimal TotalRevenue, decimal TotalExpenses, decimal Refunds, decimal OutstandingPayments);

    public interface IPaymentLogRepository
    {
        Task<PaymentLog> CreateAsync(PaymentLog paymentLog);

        // Admin — all platform logs
        Task<IEnumerable<PaymentLog>> GetAllPagedAsync(PaymentType? type, PaymentDirection? direction, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize);
        Task<int> CountAllAsync(PaymentType? type, PaymentDirection? direction, DateTime? dateFrom, DateTime? dateTo);

        // Hotel — paged (all transactions)
        Task<IEnumerable<PaymentLog>> GetHotelLogsAsync(int hotelId, PaymentType? type, PaymentDirection? direction, DateTime? dateFrom, DateTime? dateTo, bool ascending, int pageNumber, int pageSize);
        Task<int> CountHotelLogsAsync(int hotelId, PaymentType? type, PaymentDirection? direction, DateTime? dateFrom, DateTime? dateTo);

        // Hotel — unfiltered summary totals in one query
        Task<HotelPaymentSummary> GetHotelSummaryAsync(int hotelId);

        // Single record with Reservation included
        Task<PaymentLog?> GetByIdAsync(int paymentLogId);
        // Agency overview stats
        Task<decimal> GetTotalIncomingByAgencyAsync(int agencyId);
        Task<IEnumerable<MonthlyRevenue>> GetMonthlyIncomingByAgencyAsync(int agencyId, DateTime from, DateTime to);
        Task<IEnumerable<MonthlyRevenue>> GetMonthlyOutgoingByAgencyAsync(int agencyId, DateTime from, DateTime to);
        Task<IEnumerable<HotelRevenue>> GetRevenuePerHotelByAgencyAsync(int agencyId);
        Task<IEnumerable<MonthlyRevenue>> GetMonthlyIncomingByHotelAsync(int hotelId, DateTime from, DateTime to);
        Task<IEnumerable<MonthlyRevenue>> GetMonthlyOutgoingByHotelAsync(int hotelId, DateTime from, DateTime to);
        Task<IEnumerable<PaymentTypeRevenue>> GetRevenueByTypeByHotelAsync(int hotelId);
        Task<IEnumerable<MonthlyNet>> GetMonthlyNetByHotelAsync(int hotelId);
        Task<RefundImpactData> GetRefundImpactByHotelAsync(int hotelId);
        Task<decimal> GetMonthlyBookingRevenueByHotelAsync(int hotelId, int year, int month);
        Task<FinancialSummaryData> GetFinancialSummaryByHotelAsync(int hotelId);
        Task<PaymentLog> UpdateAsync(PaymentLog paymentLog);
        Task DeleteAsync(PaymentLog paymentLog);
    }
}
