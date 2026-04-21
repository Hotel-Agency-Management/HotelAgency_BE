using Booking.DTO;
using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface ITeamMemberService
    {
        Task<PaginatedResponse<TeamMemberResponse>> GetAgencyTeamMembersAsync(
            int agencyId,
            int hotelId,
            int agencyOwnerId,
            TeamMemberListRequest request);

        Task<TeamMemberResponse> CreateAgencyTeamMemberAsync(
            int agencyId,
            int hotelId,
            CreateTeamMemberRequest request);

        Task<TeamMemberResponse> AssignAgencyTeamMemberRoleAsync(
            int agencyId,
            int hotelId,
            int teamMemberId,
            AssignTeamMemberRoleRequest request);
    }
}
