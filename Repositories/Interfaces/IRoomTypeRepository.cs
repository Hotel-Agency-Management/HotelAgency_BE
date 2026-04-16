using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IRoomTypeRepository
    {
        Task<RoomType> CreateAsync(RoomType roomType);
        Task<RoomType?> GetByIdAsync(int roomTypeId);
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task<RoomType> UpdateAsync(RoomType roomType);
        Task DeleteAsync(RoomType roomType);
    }
}
