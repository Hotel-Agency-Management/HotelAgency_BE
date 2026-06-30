using Booking.DTO;
using Booking.Models;
namespace Booking.Interfaces.Services
{
    public interface IAgencyService
    {
        Task<PaginatedResponse<AgencyListItemResponse>> GetAllAgenciesAsync(AgencyListRequest request);
        Task<AgencyStatusCountsResponse> GetAgencyStatusCountsAsync();
        Task<Agency> GetAgencyProfileAsync(int agencyId);
        Task UpdateAgencyAsync(int agencyId, UpdateAgencyRequest request);
        Task<string> UpdateAgencyLogoAsync(int agencyId, IFormFile file);
        Task ChangePlanAsync(int agencyId, int planId);
        Task<AgencyGrowthResponse> GetMonthlyGrowthAsync();
    }
}
