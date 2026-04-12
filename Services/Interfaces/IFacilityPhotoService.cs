using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IFacilityPhotoService
    {
        Task<FacilityPhotoResponse> UploadPhotosAsync(int facilityId, UploadPhotosRequest request);
        Task<IEnumerable<FacilityPhotoResponse>> GetPhotosByFacilityIdAsync(int facilityId);
        Task DeletePhotoAsync(int photoId);
    }
}
