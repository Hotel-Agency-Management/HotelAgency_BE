using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories.Implementations
{
    public class RoomAmenityRepository(ApplicationDbContext _context) : IRoomAmenityRepository
    {
        public async Task<RoomAmenity> CreateAsync(RoomAmenity amenity)
        {
            await _context.RoomAmenities.AddAsync(amenity);
            await _context.SaveChangesAsync();
            return amenity;
        }

        public async Task<RoomAmenity?> GetByIdAsync(int amenityId)
            => await _context.RoomAmenities.FindAsync(amenityId);

        public async Task<IEnumerable<RoomAmenity>> GetAllAsync()
            => await _context.RoomAmenities.ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.RoomAmenities
                .AnyAsync(a => a.Name.ToLower() == name.ToLower());

        public async Task DeleteAsync(RoomAmenity amenity)
        {
            _context.RoomAmenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }
    }
}
