using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IPaymentLogService
    {
        Task<PaginatedResponse<PaymentLogItemResponse>> GetAllAsync(PaymentLogListRequest request);
        Task<PaymentLogSummaryResponse> GetHotelLogsAsync(int hotelId, bool? incoming, PaymentLogListRequest request);
        Task<PaymentLogDetailsResponse> GetDetailsAsync(int hotelId, int paymentLogId);
        Task<decimal> GetAgencyRevenueStatsAsync(int agencyId);
        Task<IReadOnlyList<MonthlyRevenueItem>> GetAgencyRevenueTrendAsync(int agencyId);
        Task<IReadOnlyList<MonthlyRevenueItem>> GetAgencyMonthlyProfitAsync(int agencyId);
        Task<IReadOnlyList<MonthlyRevenueItem>> GetAgencyMonthlyExpensesAsync(int agencyId);
        Task<IReadOnlyList<HotelRevenueItem>> GetAgencyRevenuePerHotelAsync(int agencyId);
        Task<IReadOnlyList<CashFlowItem>> GetHotelCashFlowAsync(int hotelId);
        Task<IReadOnlyList<RevenueByTypeItem>> GetHotelRevenueByTypeAsync(int hotelId);
        Task<IReadOnlyList<BalanceTrendItem>> GetHotelBalanceTrendAsync(int hotelId);
        Task<RefundImpactResponse> GetHotelRefundImpactAsync(int hotelId);
        Task<RevenueGrowthResponse> GetHotelRevenueGrowthAsync(int hotelId, int month, int year);
        Task<FinancialSummaryResponse> GetHotelFinancialSummaryAsync(int hotelId);
        Task<PaymentLogDetailsResponse> CreateAsync(int hotelId, CreatePaymentLogRequest request);
        Task<PaymentLogDetailsResponse> UpdateAsync(int hotelId, int paymentLogId, UpdatePaymentLogRequest request);
        Task DeleteAsync(int hotelId, int paymentLogId);

    }
}
