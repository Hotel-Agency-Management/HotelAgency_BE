using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IFacilityService
    {
        Task<FacilityResponse> CreateFacilityAsync(int hotelId, CreateFacilityRequest request);
        Task<FacilityResponse> GetFacilityByIdAsync(int facilityId);
        Task<IEnumerable<FacilityResponse>> GetFacilitiesByHotelIdAsync(int hotelId);
        Task<FacilityResponse> UpdateFacilityAsync(int facilityId, UpdateFacilityRequest request);
        Task DeleteFacilityAsync(int facilityId);
    }
}
