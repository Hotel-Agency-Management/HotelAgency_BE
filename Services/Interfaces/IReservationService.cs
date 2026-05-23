using Booking.DTO;
using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationResponse> CreateReservationAsync(int hotelId, int staffUserId, CreateReservationRequest request);
        Task<ReservationResponse> GetReservationByIdAsync(int hotelId, int reservationId);
        Task<PaginatedResponse<ListReservationResponse>> GetReservationsByHotelIdAsync(int hotelId, ReservationListRequest request);
        Task<ReservationResponse> UpdateReservationAsync(int hotelId, int reservationId, int staffUserId, UpdateReservationRequest request);
        Task<CancellationResponse> CancelReservationAsync(int hotelId, int reservationId, CancelReservationRequest request);
        Task<PaginatedResponse<ListReservationResponse>> GetMyReservationsAsync(int customerId, ReservationListRequest request);
        Task<ReservationResponse> GetMyReservationByIdAsync(int reservationId, int customerId);
        Task<ReservationResponse> UpdateMyReservationAsync(int reservationId, int customerId, UpdateReservationRequest request);
        Task<CancellationResponse> CancelMyReservationAsync(int reservationId, int customerId, CancelReservationRequest request);
        Task<ReservationResponse> CreateMyReservationAsync(int hotelId, ApplicationUser user, CustomerCreateReservationRequest request);
        Task<ReservationResponse> UpdateReservationStatusAsync(int hotelId, int reservationId, UpdateReservationStatusRequest request);
        Task<AgencyReservationStats> GetAgencyStatsAsync(int agencyId);
        Task<IReadOnlyList<BookingTypeDistributionItem>> GetAgencyBookingTypeDistributionAsync(int agencyId);
        Task<IReadOnlyList<ReservationStatusDistributionItem>> GetAgencyStatusDistributionAsync(int agencyId);
        Task<IReadOnlyList<RoomTypeReservationsItem>> GetAgencyReservationsByRoomTypeAsync(int agencyId);
        Task<PropertyManagerOverviewCardsResponse> GetHotelOverviewCardsAsync(int hotelId);
        Task<HotelReservationStatusDistributionResponse> GetHotelReservationStatusDistributionAsync(int hotelId);
    }
}
