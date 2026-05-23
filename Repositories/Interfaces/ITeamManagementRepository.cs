using Booking.Models;
using Microsoft.AspNetCore.Identity;

namespace Booking.Interfaces.Repositories
{
    public interface ITeamManagementRepository
    {
        Task<int> CountByAgencyAsync(int agencyId, int? excludedUserId, string? role, bool? assigned);
        Task<List<ApplicationUser>> GetByAgencyAsync(
            int agencyId,
            int? excludedUserId,
            string? role,
            bool? assigned,
            int pageNumber,
            int pageSize);

        Task<ApplicationUser?> GetByIdAndAgencyAsync(int userId, int agencyId);
        Task<int> CountByHotelAsync(int hotelId, string? role);
        Task<List<ApplicationUser>> GetByHotelAsync(int hotelId, string? role, int pageNumber, int pageSize);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> ReplaceRoleAsync(ApplicationUser user, string role);
        Task<string> GetRoleAsync(ApplicationUser user);
    }
}
