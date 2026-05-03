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
        Task<IEnumerable<string>> GetUnavailableRoomNumbersAsync(IEnumerable<int> roomIds, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null);
        Task<int> CountByYearAsync(int year);
        Task<Reservation> UpdateAsync(Reservation reservation);
    }
}
