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
    [EnsureHotelExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId:int}/hotels/{hotelId:int}/team-members")]
    public class TeamMemberController(ITeamManagementService _teamMemberService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromQuery] TeamMemberListRequest request)
        {
            var result = await _teamMemberService.GetAgencyTeamMembersAsync(
                agencyId: agencyId,
                hotelId: hotelId,
                excludedUserId: null,
                request: request);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamMember(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromBody] CreateTeamMemberRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _teamMemberService.CreateAgencyTeamMemberAsync(
                agencyId,
                hotelId,
                request);

            return CreatedAtAction(
                nameof(GetTeamMembers),
                new { agencyId, hotelId },
                result);
        }

        [HttpPut("{teamMemberId:int}/role")]
        public async Task<IActionResult> AssignRole(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromRoute] int teamMemberId,
            [FromBody] AssignTeamMemberRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _teamMemberService.AssignAgencyTeamMemberRoleAsync(
                agencyId,
                hotelId,
                teamMemberId,
                request);

            return Ok(result);
        }

        [HttpPut("{teamMemberId:int}/transfer")]
        public async Task<IActionResult> TransferTeamMember(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromRoute] int teamMemberId,
            [FromBody] TransferTeamMemberRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.SourceHotelId != hotelId)
                return BadRequest("SourceHotelId must match the route hotelId.");

            var result = await _teamMemberService.TransferAgencyTeamMemberAsync(
                agencyId,
                teamMemberId,
                request);

            return Ok(result);
        }
    }
}
