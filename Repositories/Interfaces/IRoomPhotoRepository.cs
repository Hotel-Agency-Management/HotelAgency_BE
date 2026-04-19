using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IRoomPhotoRepository
    {
        Task<RoomPhoto> CreateAsync(RoomPhoto photo);
        Task<RoomPhoto?> GetByIdAndRoomIdAsync(int photoId, int roomId);
        Task<IEnumerable<RoomPhoto>> GetAllByRoomIdAsync(int roomId);
        Task DeleteAsync(RoomPhoto photo);
    }
}
