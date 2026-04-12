using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IFacilityRepository
    {
        Task<Facility> CreateAsync(Facility facility);
        Task<Facility?> GetByIdAsync(int facilityId);
        Task<IEnumerable<Facility>> GetAllByHotelIdAsync(int hotelId);
        Task<bool> ExistsByNameAndHotelIdAsync(string name, int hotelId);
        Task<Facility> UpdateAsync(Facility facility);
        Task DeleteAsync(Facility facility);
    }
}
