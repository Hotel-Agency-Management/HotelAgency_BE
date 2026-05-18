using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IPaymentLogService
    {
        Task<PaginatedResponse<PaymentLogResponse>> GetAllAsync(PaymentLogListRequest request);
        Task<PaginatedResponse<PaymentLogResponse>> GetByHotelIdAsync(int hotelId, PaymentLogListRequest request);
        Task<PaymentLogExpenseResponse> GetIncomingByHotelIdAsync(int hotelId, PaymentLogListRequest request);
        Task<PaymentLogExpenseResponse> GetOutgoingByHotelIdAsync(int hotelId, PaymentLogListRequest request);
    }
}
