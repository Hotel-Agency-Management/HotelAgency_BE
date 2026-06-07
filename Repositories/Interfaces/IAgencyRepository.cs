using Booking.Models;
using Booking.Enums;
using Booking.DTO;
namespace Booking.Interfaces.Repositories
{
        public interface IAgencyRepository
        {
                Task<bool> ExistsByNameAsync(string agencyName);
                Task<(IReadOnlyList<Agency> Agencies, int TotalCount)> GetAllAsync(AgencyListRequest request);
                Task<Agency?> GetByIdAsync(int agencyId);
                Task AddAsync(Agency agency);
                Task UpdateAsync(Agency agency);
                Task DeleteAsync(Agency agency);
                Task UpdateStatusAsync(int agencyId, AgencyStatus status);
        }
}
