using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IRoomTypeService
    {
        Task<RoomTypeResponse> CreateRoomTypeAsync(CreateRoomTypeRequest request);
        Task<RoomTypeResponse> GetRoomTypeByIdAsync(int roomTypeId);
        Task<IEnumerable<RoomTypeResponse>> GetRoomTypesAsync();
        Task<RoomTypeResponse> UpdateRoomTypeAsync(int roomTypeId, UpdateRoomTypeRequest request);
        Task DeleteRoomTypeAsync(int roomTypeId);
    }
}
