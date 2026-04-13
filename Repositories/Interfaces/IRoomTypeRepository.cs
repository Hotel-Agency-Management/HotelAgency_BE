using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IRoomTypeRepository
    {
        Task<RoomType> CreateAsync(RoomType roomType);
        Task<RoomType?> GetByIdAsync(int roomTypeId);
        Task<RoomType?> GetByIdAndHotelIdAsync(int roomTypeId, int hotelId);
        Task<IEnumerable<RoomType>> GetAllByHotelIdAsync(int hotelId);
        Task<bool> ExistsByNameAndHotelIdAsync(string name, int hotelId);
        Task<RoomType> UpdateAsync(RoomType roomType);
        Task DeleteAsync(RoomType roomType);
    }
}
