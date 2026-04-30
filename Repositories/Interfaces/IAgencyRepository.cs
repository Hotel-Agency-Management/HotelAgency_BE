using Booking.Models;
using Booking.Enums;
namespace Booking.Interfaces.Repositories
{
        public interface IAgencyRepository
        {
                Task<bool> ExistsByNameAsync(string agencyName);
                Task<List<Agency>> GetAllAsync();
                Task<Agency?> GetByIdAsync(int agencyId);
                Task AddAsync(Agency agency);
                Task UpdateAsync(Agency agency);
                Task DeleteAsync(Agency agency);
                Task UpdateStatusAsync(int agencyId, AgencyStatus status);
        }
}
