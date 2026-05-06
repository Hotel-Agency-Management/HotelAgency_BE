using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IHotelRepository
    {
        Task<Hotel> CreateAsync(Hotel hotel);
        Task<Hotel?> GetByIdAsync(int hotelId);
        Task<Hotel?> GetByIdAndAgencyIdAsync(int hotelId, int agencyId);
        Task<IEnumerable<Hotel>> GetAllByAgencyIdAsync(int agencyId, string? search, string? location, int pageNumber, int pageSize);
        Task<int> CountByAgencyIdAsync(int agencyId, string? search, string? location);
        Task<Hotel> UpdateAsync(Hotel hotel);
    }
}
