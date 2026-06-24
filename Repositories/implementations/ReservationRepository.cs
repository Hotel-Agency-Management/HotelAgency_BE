using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class ReservationRepository(
        ApplicationDbContext _context,
        ILogger<ReservationRepository> _logger) : IReservationRepository
    {
        public async Task<Reservation> CreateAsync(Reservation reservation)
        {
            _logger.LogDebug("Creating reservation for hotel {HotelId}", reservation.HotelId);
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

        public async Task<IEnumerable<Reservation>> GetByHotelIdAsync(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo, int pageNumber, int pageSize)
            => await BuildHotelQuery(hotelId, search, status, checkInFrom, checkInTo)
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByHotelIdAsync(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo)
            => await BuildHotelQuery(hotelId, search, status, checkInFrom, checkInTo)
                .CountAsync();

        private IQueryable<Reservation> BuildHotelQuery(
            int hotelId, string? search, ReservationStatus? status,
            DateOnly? checkInFrom, DateOnly? checkInTo)
        {
            var q = _context.Reservations.Where(r => r.HotelId == hotelId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                q = q.Where(r =>
                    r.GuestFullName.ToLower().Contains(term) ||
                    r.GuestEmail.ToLower().Contains(term) ||
                    r.GuestPhone.Contains(term) ||
                    r.ReservationNumber.ToLower().Contains(term));
            }

            if (status.HasValue)
                q = q.Where(r => r.Status == status.Value);

            if (checkInFrom.HasValue)
                q = q.Where(r => r.CheckInDate >= checkInFrom.Value);

            if (checkInTo.HasValue)
                q = q.Where(r => r.CheckInDate <= checkInTo.Value);

            return q;
        }

        public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Include(r => r.Hotel)
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Reservation?> GetByIdAndCustomerIdAsync(int reservationId, int customerId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Include(r => r.Hotel)
                .Include(r => r.Customer)
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.CustomerId == customerId);

        public async Task<IEnumerable<Reservation>> GetPagedByCustomerIdAsync(
            int customerId, ReservationStatus? status, int pageNumber, int pageSize)
        {
            var q = _context.Reservations.Where(r => r.CustomerId == customerId);
            if (status.HasValue)
                q = q.Where(r => r.Status == status.Value);
            return await q
                .Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room)
                .Include(r => r.Hotel)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByCustomerIdAsync(int customerId, ReservationStatus? status)
        {
            var q = _context.Reservations.Where(r => r.CustomerId == customerId);
            if (status.HasValue)
                q = q.Where(r => r.Status == status.Value);
            return await q.CountAsync();
        }

        public async Task<IEnumerable<string>> GetUnavailableRoomNumbersAsync(
            IEnumerable<int> roomIds, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null)
        {
            var ids = roomIds.ToList();
            return await _context.ReservationRooms
                .Where(rr => ids.Contains(rr.RoomId)
                    && rr.Reservation!.Status != ReservationStatus.Cancelled
                    && rr.Reservation.Status != ReservationStatus.CheckedOut
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

        public async Task<IEnumerable<Reservation>> GetTomorrowCheckInsWithCustomerAsync(DateOnly tomorrow)
            => await _context.Reservations
                .Where(r => r.CheckInDate == tomorrow
                         && r.CustomerId != null
                         && !r.CheckInReminderSent)
                .ToListAsync();

        public async Task<int> GetTotalCountByAgencyAsync(int agencyId)
            => await _context.Reservations
                .Where(r => r.Hotel!.AgencyId == agencyId)
                .CountAsync();

        public async Task<int> GetPendingCountByAgencyAsync(int agencyId)
            => await _context.Reservations
                .Where(r => r.Hotel!.AgencyId == agencyId && r.Status == ReservationStatus.Pending)
                .CountAsync();

        public async Task<decimal> GetAverageValueByAgencyAsync(int agencyId)
        {
            var avg = await _context.Reservations
                .Where(r => r.Hotel!.AgencyId == agencyId &&
                            r.Status != ReservationStatus.Pending &&
                            r.Status != ReservationStatus.Cancelled)
                .AverageAsync(r => (decimal?)r.TotalAmount);
            return avg ?? 0m;
        }

        public async Task<IEnumerable<(ReservationSource Source, int Count)>> GetBookingSourceDistributionByAgencyAsync(
            int agencyId, DateTime from)
        {
            var rows = await _context.Reservations
                .Where(r => r.Hotel!.AgencyId == agencyId && r.CreatedAt >= from)
                .GroupBy(r => r.Source)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Source, r.Count));
        }

        public async Task<IEnumerable<(ReservationStatus Status, int Count)>> GetStatusDistributionByAgencyAsync(
            int agencyId, DateTime from)
        {
            var rows = await _context.Reservations
                .Where(r => r.Hotel!.AgencyId == agencyId && r.CreatedAt >= from)
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Status, r.Count));
        }

        public async Task<IEnumerable<RoomTypeReservationCount>> GetReservationsByRoomTypeForAgencyAsync(
            int agencyId, DateTime from)
        {
            var allTypes = await _context.RoomTypes
                .Where(rt => rt.Rooms!.Any(r => r.Hotel!.AgencyId == agencyId))
                .Select(rt => new { rt.Id, rt.Name })
                .ToListAsync();

            var counts = (await _context.ReservationRooms
                .Where(rr => rr.Room!.Hotel!.AgencyId == agencyId && rr.Reservation!.CreatedAt >= from)
                .GroupBy(rr => rr.Room!.RoomTypeId)
                .Select(g => new { RoomTypeId = g.Key, Count = g.Count() })
                .ToListAsync())
                .ToDictionary(x => x.RoomTypeId, x => x.Count);

            return allTypes
                .Select(t =>
                {
                    counts.TryGetValue(t.Id, out var count);
                    return new RoomTypeReservationCount(t.Id, t.Name, count);
                })
                .OrderByDescending(x => x.Count);
        }

        public async Task<IEnumerable<(ReservationSource Source, int Count)>> GetBookingTypeDistributionByHotelIdAsync(int hotelId)
        {
            var rows = await _context.Reservations
                .Where(r => r.HotelId == hotelId)
                .GroupBy(r => r.Source)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Source, r.Count));
        }

        public async Task<IEnumerable<(ReservationStatus Status, int Count)>> GetStatusDistributionByHotelIdAsync(int hotelId)
        {
            var rows = await _context.Reservations
                .Where(r => r.HotelId == hotelId)
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return rows.Select(r => (r.Status, r.Count));
        }

        public async Task<IEnumerable<(int Year, int Month, decimal Revenue)>> GetMonthlyRevenueByHotelAsync(
            int hotelId, DateTime from, DateTime to)
        {
            var rows = await _context.Reservations
                .Where(r => r.HotelId == hotelId
                         && r.CreatedAt >= from && r.CreatedAt < to
                         && r.Status != ReservationStatus.Cancelled)
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(r => r.TotalAmount) })
                .ToListAsync();

            return rows.Select(r => (r.Year, r.Month, r.Revenue));
        }

        public async Task<IEnumerable<(int Year, int Month, int Day, decimal Revenue)>> GetDailyRevenueByHotelAsync(
            int hotelId, DateTime from, DateTime to)
        {
            var rows = await _context.Reservations
                .Where(r => r.HotelId == hotelId
                         && r.CreatedAt >= from && r.CreatedAt < to
                         && r.Status != ReservationStatus.Cancelled)
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month, r.CreatedAt.Day })
                .Select(g => new { g.Key.Year, g.Key.Month, g.Key.Day, Revenue = g.Sum(r => r.TotalAmount) })
                .ToListAsync();

            return rows.Select(r => (r.Year, r.Month, r.Day, r.Revenue));
        }

        public async Task<HotelOverviewCards> GetHotelOverviewCardsAsync(int hotelId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var baseQuery = _context.Reservations.Where(r => r.HotelId == hotelId);

            var total = await baseQuery.CountAsync();
            var todayCheckIns = await baseQuery.CountAsync(r => r.CheckInDate == today);
            var todayCheckOuts = await baseQuery.CountAsync(r => r.CheckOutDate == today);
            var pending = await baseQuery.CountAsync(r => r.Status == ReservationStatus.Pending);

            return new HotelOverviewCards(total, todayCheckIns, todayCheckOuts, pending);
        }
    }
}
