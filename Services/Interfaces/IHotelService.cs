using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IHotelService
    {
        Task<HotelResponse> CreateHotelAsync(CreateHotelRequest request);
        Task<HotelResponse> GetHotelByIdAsync(int hotelId);
        Task<PaginatedResponse<HotelResponse>> GetHotelsByAgencyIdAsync(int agencyId, HotelListRequest request);
        Task<PaginatedResponse<HotelResponse>> GetAllHotelsAsync(HotelListRequest request);
        Task<HotelResponse> UpdateHotelAsync(int hotelId, UpdateHotelRequest request);
    }
}
