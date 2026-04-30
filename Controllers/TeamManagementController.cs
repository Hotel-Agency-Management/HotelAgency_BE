using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.AgencyOwner
{
    [ApiController]
    [Authorize(Roles = Roles.AgencyOwner)]
    [EnsureAgencyExistsForOwner]
    [Route("api/team-members")]
    public class TeamManagementController(
        ITeamManagementService _teamMemberService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers(
            [FromQuery] TeamMemberListRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.GetAgencyTeamMembersAsync(
                agencyId: agencyOwner!.AgencyId!.Value,
                excludedUserId: agencyOwner.Id,
                request: request);
            return Ok(result);
        }

        [HttpGet("available-property-managers")]
        public async Task<IActionResult> GetAvailablePropertyManagers()
        {
            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.GetAvailablePropertyManagersAsync(
                agencyOwner!.AgencyId!.Value);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamMember(
            [FromBody] CreateTeamMemberRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.CreateAgencyTeamMemberAsync(
                agencyOwner!.AgencyId!.Value,
                request);

            return CreatedAtAction(nameof(GetTeamMembers), result);
        }

        [HttpPut("{teamMemberId:int}/role")]
        public async Task<IActionResult> AssignRole(
            [FromRoute] int teamMemberId,
            [FromBody] AssignTeamMemberRoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.AssignAgencyTeamMemberRoleAsync(
                agencyOwner!.AgencyId!.Value,
                teamMemberId,
                request);

            return Ok(result);
        }
    }
}
