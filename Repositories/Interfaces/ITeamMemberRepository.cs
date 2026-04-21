using Booking.Models;
using Microsoft.AspNetCore.Identity;

namespace Booking.Interfaces.Repositories
{
    public interface ITeamMemberRepository
    {
        Task<int> CountByHotelAsync(int agencyId, int hotelId, int excludedUserId);
        Task<List<ApplicationUser>> GetByHotelAsync(int agencyId, int hotelId, int excludedUserId, int pageNumber, int pageSize);
        Task<ApplicationUser?> GetByIdAndAgencyAsync(int userId, int agencyId);
        Task<bool> HotelHasRoleAsync(int hotelId, string roleName, int? excludedUserId = null);
        Task EnsureHotelDoesNotHaveSingleAssigneeRoleAsync(int hotelId, string role, int? excludedUserId = null);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> ReplaceRoleAsync(ApplicationUser user, string role);
        Task<string> GetRoleAsync(ApplicationUser user);
    }
}
