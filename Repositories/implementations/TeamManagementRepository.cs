using Booking.Data;
using Booking.Constants;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class TeamManagementRepository(
        ApplicationDbContext _context,
        UserManager<ApplicationUser> _userManager) : ITeamManagementRepository
    {
        public Task<int> CountByHotelAsync(int agencyId, int hotelId, int? excludedUserId = null)
        {
            return _context.Users
                .CountAsync(u =>
                    u.AgencyId == agencyId &&
                    u.HotelId == hotelId &&
                    (excludedUserId == null || u.Id != excludedUserId.Value));
        }

        public Task<List<ApplicationUser>> GetByHotelAsync(
            int agencyId,
            int hotelId,
            int? excludedUserId,
            int pageNumber,
            int pageSize)
        {
            return _context.Users
                .Where(u =>
                    u.AgencyId == agencyId &&
                    u.HotelId == hotelId &&
                    (excludedUserId == null || u.Id != excludedUserId.Value))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<ApplicationUser?> GetByIdAndAgencyAsync(int userId, int agencyId)
        {
            return _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.AgencyId == agencyId);
        }

        public Task<bool> HotelHasRoleAsync(int hotelId, string roleName, int? excludedUserId = null)
        {
            return (
                from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where user.HotelId == hotelId
                    && role.Name == roleName
                    && (excludedUserId == null || user.Id != excludedUserId.Value)
                select user.Id)
                .AnyAsync();
        }

        public async Task EnsureHotelDoesNotHaveSingleAssigneeRoleAsync(
            int hotelId,
            string role,
            int? excludedUserId = null)
        {
            if (!IsSingleAssigneeHotelRole(role))
                return;

            if (await HotelHasRoleAsync(hotelId, role, excludedUserId))
                throw new HotelAlreadyHasRoleException(hotelId, role);
        }

        private static bool IsSingleAssigneeHotelRole(string role)
        {
            return role is Roles.PropertyManager or Roles.HousekeepingManager;
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> ReplaceRoleAsync(ApplicationUser user, string role)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return removeResult;
            }

            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<string> GetRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? string.Empty;
        }
    }
}
