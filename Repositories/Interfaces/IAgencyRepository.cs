using Booking.Models;
using Booking.Enums;
using Booking.DTO;
namespace Booking.Interfaces.Repositories
{
        public record MonthlyAgencyCount(int Year, int Month, int Count);

        public interface IAgencyRepository
        {
                Task<bool> ExistsByNameAsync(string agencyName);
                Task<(IReadOnlyList<Agency> Agencies, int TotalCount)> GetAllAsync(AgencyListRequest request);
                Task<AgencyStatusCountsResponse> GetStatusCountsAsync();
                Task<Agency?> GetByIdAsync(int agencyId);
                Task AddAsync(Agency agency);
                Task UpdateAsync(Agency agency);
                Task DeleteAsync(Agency agency);
                Task UpdateStatusAsync(int agencyId, AgencyStatus status);
                Task<IEnumerable<MonthlyAgencyCount>> GetMonthlyGrowthAsync(DateTime from, DateTime to);
        }
}
