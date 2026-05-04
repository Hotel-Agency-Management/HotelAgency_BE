using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface ITermsAndConditionsService
    {
        Task<TermsResponse> CreateTermsAsync(int hotelId, CreateTermsRequest dto);
        Task<IEnumerable<TermsResponse>> GetTermsByHotelIdAsync(int hotelId);
        Task<TermsResponse> GetTermsByIdAsync(int id);
        Task<TermsResponse> UpdateTermsAsync(int id, UpdateTermsRequest dto);
    }
}
