using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class RoomRepository(ApplicationDbContext _context) : IRoomRepository
    {
        public async Task<Room> CreateAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room?> GetByIdAsync(int roomId)
            => await _context.Rooms.FindAsync(roomId);

        public async Task<Room?> GetByIdAndHotelIdAsync(int roomId, int hotelId)
            => await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

        public async Task<IEnumerable<Room>> GetAllByHotelIdAsync(int hotelId)
            => await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();

        public async Task<bool> ExistsByRoomNumberAndHotelIdAsync(string roomNumber, int hotelId)
            => await _context.Rooms
                .AnyAsync(r => r.RoomNumber == roomNumber && r.HotelId == hotelId);

        public async Task<Room> UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task DeleteAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }
}
