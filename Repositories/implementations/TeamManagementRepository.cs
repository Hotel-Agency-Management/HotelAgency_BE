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
        UserManager<ApplicationUser> _userManager,
        ILogger<TeamManagementRepository> _logger) : ITeamManagementRepository
    {
        public Task<int> CountByAgencyAsync(
            int agencyId,
            int? excludedUserId,
            string? role,
            bool? assigned)
        {
            return AgencyTeamMemberQuery(agencyId, excludedUserId, role, assigned).CountAsync();
        }

        public Task<List<ApplicationUser>> GetByAgencyAsync(
            int agencyId,
            int? excludedUserId,
            string? role,
            bool? assigned,
            int pageNumber,
            int pageSize)
        {
            _logger.LogDebug("Fetching team members for agency {AgencyId}", agencyId);
            return AgencyTeamMemberQuery(agencyId, excludedUserId, role, assigned)
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

        private IQueryable<ApplicationUser> AgencyTeamMemberQuery(
            int agencyId,
            int? excludedUserId,
            string? role,
            bool? assigned)
        {
            var query = (
                from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join identityRole in _context.Roles on userRole.RoleId equals identityRole.Id
                where user.AgencyId == agencyId
                    && AllowedAgencyRoleNames.Contains(identityRole.Name!)
                    && (excludedUserId == null || user.Id != excludedUserId.Value)
                    && (role == null || identityRole.Name == role)
                select user)
                .Distinct();

            return assigned switch
            {
                true => query.Where(user => user.HotelId != null),
                false => query.Where(user => user.HotelId == null),
                _ => query
            };
        }

        public Task<int> CountByHotelAsync(int hotelId, string? role)
        {
            return HotelStaffQuery(hotelId, role).CountAsync();
        }

        public Task<List<ApplicationUser>> GetByHotelAsync(int hotelId, string? role, int pageNumber, int pageSize)
        {
            return HotelStaffQuery(hotelId, role)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private IQueryable<ApplicationUser> HotelStaffQuery(int hotelId, string? role)
        {
            return (
                from user in _context.Users
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join identityRole in _context.Roles on userRole.RoleId equals identityRole.Id
                where user.HotelId == hotelId
                    && HotelStaffRoleNames.Contains(identityRole.Name!)
                    && (role == null || identityRole.Name == role)
                select user)
                .Distinct();
        }

        private static readonly string[] HotelStaffRoleNames =
        {
            Roles.PropertyManager,
            Roles.HousekeepingManager,
            Roles.FrontDeskStaff,
            Roles.HousekeepingEmployee
        };

        private static readonly string[] AllowedAgencyRoleNames =
        {
            Roles.PropertyManager,
            Roles.FrontDeskStaff,
            Roles.HousekeepingManager,
            Roles.HousekeepingEmployee,
            Roles.Accountant,
            Roles.CustomerSupport,
            Roles.Auditor
        };

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
