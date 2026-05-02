using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation> CreateAsync(Reservation reservation);
        Task<Reservation?> GetByIdAsync(int reservationId);
        Task<Reservation?> GetByIdAndHotelIdAsync(int reservationId, int hotelId);
        Task<IEnumerable<Reservation>> GetByHotelIdAsync(int hotelId);
        Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
        Task<bool> HasOverlappingReservationAsync(int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeId = null);
        Task<int> CountByYearAsync(int year);
        Task<Reservation> UpdateAsync(Reservation reservation);
    }
}
