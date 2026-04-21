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
    [EnsureHotelBelongsToAgency]
    [Route("api/hotels/{hotelId:int}/team-members")]
    public class TeamMembersController(
        ITeamMemberService _teamMemberService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers(
            [FromRoute] int hotelId,
            [FromQuery] TeamMemberListRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);
            if (agencyOwner is null)
                return Unauthorized(Messages.Unauthorized);

            if (agencyOwner.AgencyId is null)
                return BadRequest("AgencyId is missing.");

            var result = await _teamMemberService.GetAgencyTeamMembersAsync(
                agencyId:agencyOwner.AgencyId.Value,
                hotelId:hotelId,
                agencyOwnerId:agencyOwner.Id,
                request:request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamMember(
            [FromRoute] int hotelId,
            [FromBody] CreateTeamMemberRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);
            if (agencyOwner is null)
                return Unauthorized(Messages.Unauthorized);

            if (agencyOwner.AgencyId is null)
                return BadRequest("AgencyId is missing.");

            var result = await _teamMemberService.CreateAgencyTeamMemberAsync(
                agencyOwner.AgencyId.Value,
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
            if (agencyOwner is null)
                return Unauthorized(Messages.Unauthorized);

            if (agencyOwner.AgencyId is null)
                return BadRequest("AgencyId is missing.");

            var result = await _teamMemberService.AssignAgencyTeamMemberRoleAsync(
                agencyOwner.AgencyId.Value,
                hotelId,
                teamMemberId,
                request);

            return Ok(result);
        }
    }
}
