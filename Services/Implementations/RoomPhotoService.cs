using Booking.Clients;
using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class RoomPhotoService(
        IRoomPhotoRepository _roomPhotoRepository,
        IRoomRepository _roomRepository,
        IBlobStorageService _blobStorageService,
        ISystemLogService _logService,
        ILogger<RoomPhotoService> _logger) : IRoomPhotoService
    {
        public async Task<RoomPhotoResponse> UploadPhotosAsync(int roomId, UploadRoomPhotosRequest request)
        {
            var room = await _roomRepository.GetByIdAsync(roomId)
                ?? throw new RoomNotFoundException(roomId);

            var photoUrl = await _blobStorageService.UploadAsync(request.Photo);

            var photo = new RoomPhoto
            {
                RoomId = roomId,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _roomPhotoRepository.CreateAsync(photo);
            _logger.LogInformation("Photo uploaded for room {RoomId}", roomId);
            return new RoomPhotoResponse(saved);
        }

        public async Task<IEnumerable<RoomPhotoResponse>> GetPhotosByRoomIdAsync(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId)
                ?? throw new RoomNotFoundException(roomId);

            var photos = await _roomPhotoRepository.GetAllByRoomIdAsync(roomId);
            return photos.Select(p => new RoomPhotoResponse(p));
        }

        public async Task DeletePhotoAsync(int roomId, int photoId)
        {
            var photo = await _roomPhotoRepository.GetByIdAndRoomIdAsync(photoId, roomId)
                ?? throw new RoomPhotoNotFoundException(photoId);

            await _blobStorageService.DeleteAsync(photo.PhotoUrl);
            await _roomPhotoRepository.DeleteAsync(photo);
            await _logService.LogAsync(
                SystemLogActions.RoomPhotoDeleted,
                SystemLogEntityTypes.RoomPhoto,
                photoId,
                string.Format(SystemLogMessages.RoomPhotoDeleted, photo.Id));
            _logger.LogInformation("Photo {PhotoId} deleted for room {RoomId}", photoId, roomId);
        }
    }
}
