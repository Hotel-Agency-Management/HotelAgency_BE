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
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId:int}/team-members")]
    public class TeamManagementController(
        ITeamManagementService _teamMemberService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers(
            [FromRoute] int hotelId,
            [FromQuery] TeamMemberListRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.GetAgencyTeamMembersAsync(
                agencyId: agencyOwner!.AgencyId!.Value,
                hotelId: hotelId,
                excludedUserId: agencyOwner.Id,
                request: request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamMember(
            [FromRoute] int hotelId,
            [FromBody] CreateTeamMemberRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.CreateAgencyTeamMemberAsync(
                agencyOwner!.AgencyId!.Value,
                hotelId,
                request);

            return CreatedAtAction(nameof(GetTeamMembers), new { hotelId }, result);
        }

        [HttpPut("{teamMemberId:int}/role")]
        public async Task<IActionResult> AssignRole(
            [FromRoute] int hotelId,
            [FromRoute] int teamMemberId,
            [FromBody] AssignTeamMemberRoleRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.AssignAgencyTeamMemberRoleAsync(
                agencyOwner!.AgencyId!.Value,
                hotelId,
                teamMemberId,
                request);

            return Ok(result);
        }

        [HttpPut("{teamMemberId:int}/transfer")]
        public async Task<IActionResult> TransferTeamMember(
            [FromRoute] int hotelId,
            [FromRoute] int teamMemberId,
            [FromBody] TransferTeamMemberRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.SourceHotelId != hotelId)
                return BadRequest("SourceHotelId must match the route hotelId.");

            var agencyOwner = await _userManager.GetUserAsync(User);

            var result = await _teamMemberService.TransferAgencyTeamMemberAsync(
                agencyOwner!.AgencyId!.Value,
                teamMemberId,
                request);

            return Ok(result);
        }

    }
}
