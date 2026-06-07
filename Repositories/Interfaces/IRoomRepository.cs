using Booking.DTO;
using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(Room room);
        Task<Room?> GetByIdAsync(int roomId);
        Task<Room?> GetByIdAndHotelIdAsync(int roomId, int hotelId);
        Task<(IReadOnlyList<Room> Rooms, int TotalCount)> GetFilteredByHotelIdAsync(int hotelId, GetHotelRoomsRequest request);
        Task<bool> ExistsByRoomNumberAndHotelIdAsync(string roomNumber, int hotelId);
        Task<IEnumerable<Room>> GetByRoomNumbersAndHotelIdAsync(IEnumerable<string> roomNumbers, int hotelId);
        Task<Room> UpdateAsync(Room room);
        Task DeleteAsync(Room room);
        Task<Dictionary<RoomStatus, int>> GetStatusCountsByHotelIdAsync(int hotelId);
    }
}
