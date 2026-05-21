using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IPaymentLogService
    {
        Task<PaginatedResponse<PaymentLogItemResponse>> GetAllAsync(PaymentLogListRequest request);
        Task<PaymentLogSummaryResponse> GetHotelLogsAsync(int hotelId, bool? incoming, PaymentLogListRequest request);
        Task<PaymentLogDetailsResponse> GetDetailsAsync(int hotelId, int paymentLogId);
        Task<decimal> GetAgencyRevenueStatsAsync(int agencyId);
    }
}
