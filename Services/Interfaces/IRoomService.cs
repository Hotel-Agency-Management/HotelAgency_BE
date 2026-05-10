using Booking.DTO;
namespace Booking.Interfaces.Services
{
    public interface IRoomService
    {
        Task<RoomResponse> CreateRoomAsync(int hotelId, CreateRoomRequest request);
        Task<RoomResponse> GetRoomByIdAsync(int hotelId, int roomId);
        Task<HotelRoomResponse> GetPublicRoomByIdAsync(int hotelId, int roomId);
        Task<PaginatedResponse<HotelRoomResponse>> GetFilteredRoomsByHotelIdAsync(int hotelId, GetHotelRoomsRequest request);
        Task<RoomResponse> UpdateRoomAsync(int hotelId, int roomId, UpdateRoomRequest request);
        Task DeleteRoomAsync(int hotelId, int roomId);
    }
}
