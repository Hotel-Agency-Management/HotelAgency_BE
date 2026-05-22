using Booking.Constants;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId}/overview")]
    public class PropertyManagerOverviewController(IRoomService _roomService) : ControllerBase
    {
        [HttpGet("room-status-distribution")]
        public async Task<IActionResult> GetRoomStatusDistribution([FromRoute] int hotelId)
        {
            var result = await _roomService.GetRoomStatusDistributionAsync(hotelId);
            return Ok(result);
        }
    }
}
