using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [EnsureAgencyExistsForOwnerAttribute]
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId:int}/staff")]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}," +
                       $"{Roles.FrontDeskStaff},{Roles.HousekeepingManager}")]
    public class HotelStaffController(ITeamManagementService _teamManagementService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetHotelStaff(
            [FromRoute] int hotelId,
            [FromQuery] TeamMemberListRequest request)
        {
            var result = await _teamManagementService.GetHotelStaffAsync(hotelId, request);
            return Ok(result);
        }
    }
}
