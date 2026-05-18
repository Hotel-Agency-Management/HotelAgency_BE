using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IPaymentLogRepository
    {
        Task<PaymentLog> CreateAsync(PaymentLog paymentLog);
        Task<IEnumerable<PaymentLog>> GetAllAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
        Task<int> CountAllAsync(PaymentType? type, DateTime? dateFrom, DateTime? dateTo);
        Task<IEnumerable<PaymentLog>> GetByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
        Task<int> CountByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);
        Task<IEnumerable<PaymentLog>> GetIncomingByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
        Task<int> CountIncomingByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);
        Task<decimal> SumIncomingByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);
        Task<IEnumerable<PaymentLog>> GetOutgoingByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
        Task<int> CountOutgoingByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);
        Task<decimal> SumOutgoingByHotelIdAsync(int hotelId, PaymentType? type, DateTime? dateFrom, DateTime? dateTo);
    }
}
