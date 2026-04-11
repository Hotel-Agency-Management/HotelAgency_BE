using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface IHotelService
    {
        Task<HotelResponse> CreateHotelAsync(CreateHotelRequest request);
        Task<HotelResponse> GetHotelByIdAsync(int hotelId);
        Task<IEnumerable<HotelResponse>> GetHotelsByAgencyIdAsync(int agencyId);
        Task<HotelResponse> UpdateHotelAsync(int hotelId, UpdateHotelRequest request);
        Task DeleteHotelAsync(int hotelId);
    }
}
