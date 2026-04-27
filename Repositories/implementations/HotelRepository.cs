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

        public async Task<IEnumerable<Hotel>> GetAllByAgencyIdAsync(int agencyId)
            => await _context.Hotels
                .Where(h => h.AgencyId == agencyId)
                .ToListAsync();

        public async Task<Hotel> UpdateAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }
    }
}
