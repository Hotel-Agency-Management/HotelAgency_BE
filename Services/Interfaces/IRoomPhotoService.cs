using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IRoomPhotoService
    {
        Task<RoomPhotoResponse> UploadPhotosAsync(int roomId, UploadRoomPhotosRequest request);
        Task<IEnumerable<RoomPhotoResponse>> GetPhotosByRoomIdAsync(int roomId);
        Task DeletePhotoAsync(int roomId, int photoId);
    }
}
