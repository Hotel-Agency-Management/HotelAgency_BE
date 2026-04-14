using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId}/hotels/{hotelId}/rooms")]
    public class RoomController(IRoomService _roomService) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}, {Roles.PropertyManager}")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom(
            [FromRoute] int hotelId,
            [FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _roomService.CreateRoomAsync(hotelId, request);
            return Created(string.Empty, room);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}, {Roles.PropertyManager}, {Roles.HousekeepingManager}")]
        [HttpGet]
        public async Task<IActionResult> GetRoomsByHotel([FromRoute] int hotelId)
        {
            var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);
            return Ok(rooms);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}, {Roles.PropertyManager}")]
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomById(
            [FromRoute] int hotelId,
            [FromRoute] int roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(hotelId, roomId);
            return Ok(room);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}, {Roles.PropertyManager}")]
        [HttpPut("{roomId}")]
        public async Task<IActionResult> UpdateRoom(
            [FromRoute] int hotelId,
            [FromRoute] int roomId,
            [FromBody] UpdateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _roomService.UpdateRoomAsync(hotelId, roomId, request);
            return Ok(updated);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}, {Roles.PropertyManager}")]
        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom(
            [FromRoute] int hotelId,
            [FromRoute] int roomId)
        {
            await _roomService.DeleteRoomAsync(hotelId, roomId);
            return NoContent();
        }
    }
}
