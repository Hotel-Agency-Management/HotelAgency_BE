using Booking.DTO;
using Booking.Models;

namespace Booking.Interfaces.Services
{
    public interface ITeamManagementService
    {
        Task<PaginatedResponse<TeamMemberResponse>> GetAgencyTeamMembersAsync(
            int agencyId,
            int hotelId,
            int? excludedUserId,
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

        Task<TeamMemberResponse> TransferAgencyTeamMemberAsync(
            int agencyId,
            int teamMemberId,
            TransferTeamMemberRequest request);
    }
}
