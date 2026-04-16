using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IFacilityPhotoRepository
    {
        Task<FacilityPhoto> CreateAsync(FacilityPhoto photo);
        Task<FacilityPhoto?> GetByIdAsync(int photoId);
        Task<IEnumerable<FacilityPhoto>> GetAllByFacilityIdAsync(int facilityId);
        Task DeleteAsync(FacilityPhoto photo);
    }
}
