using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface ITermsAndConditionsRepository
    {
        Task<TermsAndConditions> CreateAsync(TermsAndConditions terms);
        Task<TermsAndConditions?> GetByIdAsync(int id);
        Task<IEnumerable<TermsAndConditions>> GetAllByHotelIdAsync(int hotelId);
        Task<TermsAndConditions> UpdateAsync(TermsAndConditions terms);
        Task SetAllToInactiveForHotelAsync(int hotelId, int? excludeId = null);
    }
}
