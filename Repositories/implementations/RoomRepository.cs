using Booking.Data;
using Booking.DTO;
using Booking.Enums;
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
            return await GetByIdAsync(room.Id) ?? room;
        }

        public async Task<Room?> GetByIdAsync(int roomId)
            => await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Amenities)
                .FirstOrDefaultAsync(r => r.Id == roomId);

        public async Task<Room?> GetByIdAndHotelIdAsync(int roomId, int hotelId)
            => await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Amenities)
                .FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

        public async Task<(IReadOnlyList<Room> Rooms, int TotalCount)> GetFilteredByHotelIdAsync(
            int hotelId, GetHotelRoomsRequest request)
        {
            var query = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Amenities)
                .Where(r => r.HotelId == hotelId)
                .AsQueryable();

            if (request.CheckIn.HasValue && request.CheckOut.HasValue)
            {
                var checkIn = request.CheckIn.Value;
                var checkOut = request.CheckOut.Value;
                query = query.Where(r => !_context.Reservations.Any(res =>
                    (res.Status == ReservationStatus.Confirmed || res.Status == ReservationStatus.CheckedIn) &&
                    res.CheckInDate < checkOut &&
                    res.CheckOutDate > checkIn &&
                    res.ReservationRooms.Any(rr => rr.RoomId == r.Id)));
            }

            if (request.Guests.HasValue)
                query = query.Where(r => r.Capacity >= request.Guests.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(r => r.DailyPrice <= request.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var text = request.SearchText.Trim().ToLower();
                query = query.Where(r =>
                    r.RoomType!.Name.ToLower().Contains(text) ||
                    (r.Description != null && r.Description.ToLower().Contains(text)) ||
                    r.Amenities.Any(a => a.Name.ToLower().Contains(text)));
            }

            if (request.RoomTypeId.HasValue)
                query = query.Where(r => r.RoomTypeId == request.RoomTypeId.Value);

            var totalCount = await query.CountAsync();

            var rooms = await query
                .OrderBy(r => r.DailyPrice)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (rooms, totalCount);
        }

        public async Task<bool> ExistsByRoomNumberAndHotelIdAsync(string roomNumber, int hotelId)
            => await _context.Rooms
                .AnyAsync(r => r.RoomNumber == roomNumber && r.HotelId == hotelId);

        public async Task<IEnumerable<Room>> GetByRoomNumbersAndHotelIdAsync(IEnumerable<string> roomNumbers, int hotelId)
        {
            var numbers = roomNumbers.ToList();
            return await _context.Rooms
                .Where(r => numbers.Contains(r.RoomNumber) && r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<Room> UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(room.Id) ?? room;
        }

        public async Task DeleteAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }
}
