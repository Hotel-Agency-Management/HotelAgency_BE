using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class RoomPhotoRepository(ApplicationDbContext _context) : IRoomPhotoRepository
    {
        public async Task<RoomPhoto> CreateAsync(RoomPhoto photo)
        {
            await _context.RoomPhotos.AddAsync(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<RoomPhoto?> GetByIdAndRoomIdAsync(int photoId, int roomId)
            => await _context.RoomPhotos
                .FirstOrDefaultAsync(p => p.Id == photoId && p.RoomId == roomId);

        public async Task<IEnumerable<RoomPhoto>> GetAllByRoomIdAsync(int roomId)
            => await _context.RoomPhotos
                .Where(p => p.RoomId == roomId)
                .ToListAsync();

        public async Task DeleteAsync(RoomPhoto photo)
        {
            _context.RoomPhotos.Remove(photo);
            await _context.SaveChangesAsync();
        }
    }
}
