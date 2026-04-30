using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnsureAgencyExistsForAdmin]
    [Route("api/admin/agencies/{agencyId:int}/team-members")]
    public class TeamManagementController(ITeamManagementService _teamMemberService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers(
            [FromRoute] int agencyId,
            [FromQuery] TeamMemberListRequest request)
        {
            var result = await _teamMemberService.GetAgencyTeamMembersAsync(
                agencyId: agencyId,
                excludedUserId: null,
                request: request);

            return Ok(result);
        }

        [HttpGet("available-property-managers")]
        public async Task<IActionResult> GetAvailablePropertyManagers([FromRoute] int agencyId)
        {
            var result = await _teamMemberService.GetAvailablePropertyManagersAsync(agencyId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamMember(
            [FromRoute] int agencyId,
            [FromBody] CreateTeamMemberRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _teamMemberService.CreateAgencyTeamMemberAsync(
                agencyId,
                request);

            return CreatedAtAction(
                nameof(GetTeamMembers),
                new { agencyId },
                result);
        }

        [HttpPut("{teamMemberId:int}/role")]
        public async Task<IActionResult> AssignRole(
            [FromRoute] int agencyId,
            [FromRoute] int teamMemberId,
            [FromBody] AssignTeamMemberRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _teamMemberService.AssignAgencyTeamMemberRoleAsync(
                agencyId,
                teamMemberId,
                request);

            return Ok(result);
        }
    }
}
