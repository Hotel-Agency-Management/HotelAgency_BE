using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation> CreateAsync(Reservation reservation);
        Task<Reservation?> GetByIdAsync(int reservationId);
        Task<Reservation?> GetByIdAndHotelIdAsync(int reservationId, int hotelId);
        Task<IEnumerable<Reservation>> GetByHotelIdAsync(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo, int pageNumber, int pageSize);
        Task<int> CountByHotelIdAsync(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo);
        Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
        Task<Reservation?> GetByIdAndCustomerIdAsync(int reservationId, int customerId);
        Task<IEnumerable<Reservation>> GetPagedByCustomerIdAsync(int customerId, ReservationStatus? status, int pageNumber, int pageSize);
        Task<int> CountByCustomerIdAsync(int customerId, ReservationStatus? status);
        Task<IEnumerable<string>> GetUnavailableRoomNumbersAsync(IEnumerable<int> roomIds, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null);
        Task<int> CountByYearAsync(int year);
        Task<Reservation> UpdateAsync(Reservation reservation);

        // Agency overview stats
        Task<int> GetTotalCountByAgencyAsync(int agencyId);
        Task<int> GetPendingCountByAgencyAsync(int agencyId);
        Task<decimal> GetAverageValueByAgencyAsync(int agencyId);
    }
}
