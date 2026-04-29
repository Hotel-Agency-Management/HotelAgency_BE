using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.PropertyManager}")]
    [Route("api/room-types")]
    public class RoomTypeController(IRoomTypeService _roomTypeService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRoomTypes()
        {
            var roomTypes = await _roomTypeService.GetRoomTypesAsync();
            return Ok(roomTypes);
        }
    }
}
