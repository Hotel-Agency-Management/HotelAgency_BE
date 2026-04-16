using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IRoomAmenityRepository
    {
        Task<RoomAmenity> CreateAsync(RoomAmenity amenity);
        Task<RoomAmenity?> GetByIdAsync(int amenityId);
        Task<IEnumerable<RoomAmenity>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task DeleteAsync(RoomAmenity amenity);
    }
}
