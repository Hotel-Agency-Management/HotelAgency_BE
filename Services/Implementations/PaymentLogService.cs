using Booking.DTO;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
namespace Booking.Services
{
    public class PaymentLogService(IPaymentLogRepository _paymentLogRepository) : IPaymentLogService
    {
        public async Task<PaginatedResponse<PaymentLogResponse>> GetAllAsync(PaymentLogListRequest request)
        {
            var items = await _paymentLogRepository.GetAllAsync(
                request.Type, request.DateFrom, request.DateTo,
                request.PageNumber, request.PageSize);
            var total = await _paymentLogRepository.CountAllAsync(
                request.Type, request.DateFrom, request.DateTo);

            return BuildPaginatedResponse(items, total, request.PageNumber, request.PageSize);
        }

        public async Task<PaginatedResponse<PaymentLogResponse>> GetByHotelIdAsync(
            int hotelId, PaymentLogListRequest request)
        {
            var items = await _paymentLogRepository.GetByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo,
                request.PageNumber, request.PageSize);
            var total = await _paymentLogRepository.CountByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo);

            return BuildPaginatedResponse(items, total, request.PageNumber, request.PageSize);
        }

        public async Task<PaymentLogExpenseResponse> GetIncomingByHotelIdAsync(
            int hotelId, PaymentLogListRequest request)
        {
            var items = await _paymentLogRepository.GetIncomingByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo,
                request.PageNumber, request.PageSize);
            var total = await _paymentLogRepository.CountIncomingByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo);
            var sum = await _paymentLogRepository.SumIncomingByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo);

            return BuildExpenseResponse(items, total, sum, request.PageNumber, request.PageSize);
        }

        public async Task<PaymentLogExpenseResponse> GetOutgoingByHotelIdAsync(
            int hotelId, PaymentLogListRequest request)
        {
            var items = await _paymentLogRepository.GetOutgoingByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo,
                request.PageNumber, request.PageSize);
            var total = await _paymentLogRepository.CountOutgoingByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo);
            var sum = await _paymentLogRepository.SumOutgoingByHotelIdAsync(
                hotelId, request.Type, request.DateFrom, request.DateTo);

            return BuildExpenseResponse(items, total, sum, request.PageNumber, request.PageSize);
        }

        private static PaginatedResponse<PaymentLogResponse> BuildPaginatedResponse(
            IEnumerable<PaymentLog> items, int total, int pageNumber, int pageSize)
            => new()
            {
                Items = items.Select(ToResponse).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };

        private static PaymentLogResponse ToResponse(PaymentLog p) => new()
        {
            Id = p.Id,
            ReservationId = p.ReservationId,
            Amount = p.Amount,
            Type = p.Type.ToString(),
            From = p.From,
            To = p.To,
            CreatedAt = p.CreatedAt
        };

        private static PaymentLogExpenseResponse BuildExpenseResponse(
            IEnumerable<PaymentLog> items, int total, decimal sum, int pageNumber, int pageSize)
            => new()
            {
                Items = items.Select(ToResponse).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                TotalAmount = sum
            };

    }
}
