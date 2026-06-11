using Booking.Clients;
using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class FacilityPhotoService(
        IFacilityPhotoRepository _photoRepository,
        IFacilityRepository _facilityRepository,
        IBlobStorageService _blobStorageService,
        ISystemLogService _logService,
        ILogger<FacilityPhotoService> _logger) : IFacilityPhotoService
    {
        public async Task<FacilityPhotoResponse> UploadPhotosAsync(int facilityId, UploadPhotosRequest request)
        {

            var facility = await _facilityRepository.GetByIdAsync(facilityId)
                ?? throw new FacilityNotFoundException(facilityId);

            var photoUrl = await _blobStorageService.UploadAsync(request.Photo);

            var photo = new FacilityPhoto
            {
                FacilityId = facilityId,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow
            };

            var response = await _photoRepository.CreateAsync(photo);
            _logger.LogInformation("Photo uploaded for facility {FacilityId}", facilityId);
            return new FacilityPhotoResponse(response);
        }

        public async Task<IEnumerable<FacilityPhotosResponse>> GetPhotosByFacilityIdAsync(int facilityId)
        {
            var facility = await _facilityRepository.GetByIdAsync(facilityId)
                ?? throw new FacilityNotFoundException(facilityId);

            var photos = await _photoRepository.GetAllByFacilityIdAsync(facilityId);
            return photos.Select(p => new FacilityPhotosResponse(p));
        }

        public async Task<FacilityPhotoResponse> GetPhotoByIdAsync(int photoId)
        {
            var photo = await _photoRepository.GetByIdAsync(photoId)
                ?? throw new FacilityPhotoNotFoundException(photoId);

            return new FacilityPhotoResponse(photo);
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            var photo = await _photoRepository.GetByIdAsync(photoId)
                ?? throw new FacilityPhotoNotFoundException(photoId);

            await _blobStorageService.DeleteAsync(photo.PhotoUrl);
            await _photoRepository.DeleteAsync(photo);

            await _logService.LogAsync(
                SystemLogActions.FacilityPhotoDeleted,
                SystemLogEntityTypes.FacilityPhoto,
                photoId,
                string.Format(SystemLogMessages.FacilityPhotoDeleted, photo.FacilityId));
            _logger.LogInformation("Photo {PhotoId} deleted for facility {FacilityId}", photoId, photo.FacilityId);

        }
    }
}
