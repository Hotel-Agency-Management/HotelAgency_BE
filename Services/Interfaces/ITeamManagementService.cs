using Booking.DTO;
using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface ITeamManagementService
    {
        Task<PaginatedResponse<TeamMemberResponse>> GetAgencyTeamMembersAsync(
            int agencyId,
            int? excludedUserId,
            TeamMemberListRequest request);

        Task<TeamMemberResponse> CreateAgencyTeamMemberAsync(
            int agencyId,
            CreateTeamMemberRequest request);

        Task<PaginatedResponse<TeamMemberResponse>> GetHotelStaffAsync(int hotelId, TeamMemberListRequest request);

        Task<TeamMemberResponse> AssignAgencyTeamMemberRoleAsync(
            int agencyId,
            int teamMemberId,
            AssignTeamMemberRoleRequest request);
    }
}
