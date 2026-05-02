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
                .Include(r => r.Room)
                .Include(r => r.Hotel)
                .Include(r => r.Customer)
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

        public async Task<Reservation?> GetByIdAndHotelIdAsync(int reservationId, int hotelId)
            => await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Hotel)
                .Include(r => r.Customer)
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.HotelId == hotelId);

        public async Task<IEnumerable<Reservation>> GetByHotelIdAsync(int hotelId)
            => await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId)
            => await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Hotel)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<bool> HasOverlappingReservationAsync(int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeId = null)
            => await _context.Reservations
                .Where(r => r.RoomId == roomId
                    && (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn)
                    && r.CheckInDate < checkOut
                    && r.CheckOutDate > checkIn
                    && (excludeId == null || r.Id != excludeId))
                .AnyAsync();

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
