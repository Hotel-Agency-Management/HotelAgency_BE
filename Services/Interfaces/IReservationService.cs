using Booking.DTO;
using Booking.Enums;

namespace Booking.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationResponse> CreateReservationAsync(int hotelId, int staffUserId, CreateReservationRequest request);
        Task<ReservationResponse> GetReservationByIdAsync(int hotelId, int reservationId);
        Task<PaginatedResponse<ListReservationResponse>> GetReservationsByHotelIdAsync(int hotelId, ReservationListRequest request);
        Task<ReservationResponse> UpdateReservationAsync(int hotelId, int reservationId, int staffUserId, UpdateReservationRequest request);
        Task<CancellationResponse> CancelReservationAsync(int hotelId, int reservationId, CancelReservationRequest request);
    }
}
