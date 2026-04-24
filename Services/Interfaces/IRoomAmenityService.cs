using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IRoomAmenityService
    {
        Task<RoomAmenityResponse> CreateAmenityAsync(CreateRoomAmenityRequest request);
        Task<RoomAmenityResponse> GetAmenityByIdAsync(int amenityId);
        Task<IEnumerable<RoomAmenityResponse>> GetAllAmenitiesAsync();
        Task DeleteAmenityAsync(int amenityId);
    }
}
