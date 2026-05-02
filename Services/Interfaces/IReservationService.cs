using Booking.DTO;
using Booking.Enums;

namespace Booking.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationResponse> CreateReservationAsync(int hotelId, int staffUserId, CreateReservationRequest request);
        Task<ReservationResponse> GetReservationByIdAsync(int hotelId, int reservationId);
        Task<IEnumerable<ListReservationResponse>> GetReservationsByHotelIdAsync(int hotelId);
        Task<ReservationResponse> UpdateReservationAsync(int hotelId, int reservationId, int staffUserId, UpdateReservationRequest request);
        Task<ReservationResponse> UpdateReservationStatusAsync(int hotelId, int reservationId, ReservationStatus newStatus);
    }
}
