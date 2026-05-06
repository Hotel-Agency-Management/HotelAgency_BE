using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class HotelRepository(ApplicationDbContext _context) : IHotelRepository
    {
        public async Task<Hotel> CreateAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<Hotel?> GetByIdAsync(int hotelId)
            => await _context.Hotels.FindAsync(hotelId);

        public async Task<Hotel?> GetByIdAndAgencyIdAsync(int hotelId, int agencyId)
            => await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == hotelId && h.AgencyId == agencyId);

        public async Task<IEnumerable<Hotel>> GetAllByAgencyIdAsync(
            int agencyId, string? search, string? location, int pageNumber, int pageSize)
            => await BuildQuery(agencyId, search, location)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByAgencyIdAsync(int agencyId, string? search, string? location)
            => await BuildQuery(agencyId, search, location).CountAsync();

        private IQueryable<Hotel> BuildQuery(int agencyId, string? search, string? location)
        {
            var query = _context.Hotels.Where(h => h.AgencyId == agencyId);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(h =>
                    h.City.Contains(location) || h.Country.Contains(location));

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(h =>
                    h.Name.Contains(search) ||
                    h.Facilities.Any(f => f.Name.Contains(search)) ||
                    _context.Agencies.Any(a => a.Id == h.AgencyId && a.AgencyName.Contains(search)));

            return query;
        }

        public async Task<Hotel> UpdateAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }
    }
}
