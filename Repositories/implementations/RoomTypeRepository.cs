using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class RoomTypeRepository(ApplicationDbContext _context) : IRoomTypeRepository
    {
        public async Task<RoomType> CreateAsync(RoomType roomType)
        {
            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();
            return roomType;
        }

        public async Task<RoomType?> GetByIdAsync(int roomTypeId)
            => await _context.RoomTypes.FindAsync(roomTypeId);

        public async Task<IEnumerable<RoomType>> GetAllAsync()
            => await _context.RoomTypes
                .ToListAsync();

        public async Task<bool> ExistsByNameAsync(string name)
            => await _context.RoomTypes
                .AnyAsync(r => r.Name == name);

        public async Task<RoomType> UpdateAsync(RoomType roomType)
        {
            _context.RoomTypes.Update(roomType);
            await _context.SaveChangesAsync();
            return roomType;
        }

        public async Task DeleteAsync(RoomType roomType)
        {
            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
        }
    }
}
