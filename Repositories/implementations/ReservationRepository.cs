using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class ReservationRepository(ApplicationDbContext _context) : IReservationRepository
    {
        public async Task<Reservation> CreateAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(reservation.Id) ?? reservation;
        }

        public async Task<Reservation?> GetByIdAsync(int reservationId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Include(r => r.Hotel)
                .Include(r => r.Customer)
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

        public async Task<Reservation?> GetByIdAndHotelIdAsync(int reservationId, int hotelId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Include(r => r.Hotel)
                .Include(r => r.Customer)
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.HotelId == hotelId);

        public async Task<IEnumerable<Reservation>> GetByHotelIdAsync(int hotelId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Include(r => r.Hotel)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<string>> GetUnavailableRoomNumbersAsync(
            IEnumerable<int> roomIds, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null)
        {
            var ids = roomIds.ToList();
            return await _context.ReservationRooms
                .Where(rr => ids.Contains(rr.RoomId)
                    && (rr.Reservation!.Status == ReservationStatus.Confirmed
                        || rr.Reservation.Status == ReservationStatus.CheckedIn)
                    && rr.Reservation.CheckInDate < checkOut
                    && rr.Reservation.CheckOutDate > checkIn
                    && (excludeReservationId == null || rr.ReservationId != excludeReservationId))
                .Select(rr => rr.Room!.RoomNumber)
                .Distinct()
                .ToListAsync();
        }

        public async Task<int> CountByYearAsync(int year)
            => await _context.Reservations
                .CountAsync(r => r.CreatedAt >= new DateTime(year, 1, 1)
                              && r.CreatedAt < new DateTime(year + 1, 1, 1));

        public async Task<Reservation> UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(reservation.Id) ?? reservation;
        }
    }
}
