using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IRoomRepository
    {
        Task<Room> CreateAsync(Room room);
        Task<Room?> GetByIdAsync(int roomId);
        Task<Room?> GetByIdAndHotelIdAsync(int roomId, int hotelId);
        Task<IEnumerable<Room>> GetAllByHotelIdAsync(int hotelId);
        Task<bool> ExistsByRoomNumberAndHotelIdAsync(string roomNumber, int hotelId);
        Task<Room> UpdateAsync(Room room);
        Task DeleteAsync(Room room);
    }
}
