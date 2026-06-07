using Booking.DTO;
using Booking.Models;
namespace Booking.Interfaces.Services
{
    public interface IAgencyService
    {
        Task<PaginatedResponse<AgencyListItemResponse>> GetAllAgenciesAsync(AgencyListRequest request);
        Task<Agency> GetAgencyProfileAsync(int agencyId);
        Task UpdateAgencyAsync(int agencyId, UpdateAgencyRequest request);
        Task<string> UpdateAgencyLogoAsync(int agencyId, IFormFile file);
    }
}
