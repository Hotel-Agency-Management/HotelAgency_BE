using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IFacilityPhotoService
    {
        Task<FacilityPhotoResponse> UploadPhotosAsync(int facilityId, UploadPhotosRequest request);
        Task<IEnumerable<FacilityPhotosResponse>> GetPhotosByFacilityIdAsync(int facilityId);
        Task<FacilityPhotoResponse> GetPhotoByIdAsync(int photoId);
        Task DeletePhotoAsync(int photoId);
    }
}
