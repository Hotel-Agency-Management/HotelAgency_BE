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
    [EnsureHotelExistsForAdmin]
    [Route("api/admin/agencies/{agencyId:int}/hotels/{hotelId:int}/staff")]
    public class HotelStaffController(ITeamManagementService _teamManagementService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetHotelStaff(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromQuery] TeamMemberListRequest request)
        {
            var result = await _teamManagementService.GetHotelStaffAsync(hotelId, request);
            return Ok(result);
        }
    }
}
