using Booking.Clients;
using Booking.Constants;
using Booking.DTO;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services.Implementations
{
    public class FacilityPhotoService(
        IFacilityPhotoRepository _photoRepository,
        IFacilityRepository _facilityRepository,
        IBlobStorageService _blobStorageService) : IFacilityPhotoService
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
            return new FacilityPhotoResponse(response);
        }

        public async Task<IEnumerable<FacilityPhotoResponse>> GetPhotosByFacilityIdAsync(int facilityId)
        {
            var facility = await _facilityRepository.GetByIdAsync(facilityId)
                ?? throw new FacilityNotFoundException(facilityId);

            var photos = await _photoRepository.GetAllByFacilityIdAsync(facilityId);
            return photos.Select(p => new FacilityPhotoResponse(p));
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            var photo = await _photoRepository.GetByIdAsync(photoId)
                ?? throw new FacilityPhotoNotFoundException(photoId);

            await _blobStorageService.DeleteAsync(photo.PhotoUrl);
            await _photoRepository.DeleteAsync(photo);
        }
    }
}
