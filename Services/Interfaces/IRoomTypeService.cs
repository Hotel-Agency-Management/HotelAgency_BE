using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IRoomTypeService
    {
        Task<RoomTypeResponse> CreateRoomTypeAsync(int hotelId, CreateRoomTypeRequest request);
        Task<RoomTypeResponse> GetRoomTypeByIdAsync(int hotelId, int roomTypeId);
        Task<IEnumerable<RoomTypeResponse>> GetRoomTypesByHotelIdAsync(int hotelId);
        Task<RoomTypeResponse> UpdateRoomTypeAsync(int hotelId, int roomTypeId, UpdateRoomTypeRequest request);
        Task DeleteRoomTypeAsync(int hotelId, int roomTypeId);
    }
}
