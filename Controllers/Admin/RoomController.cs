using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking.Filters;


namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [EnsureAgencyExistsForAdminAttribute]
    [EnsureHotelExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId}/hotels/{hotelId}/rooms")]
    public class RoomController(IRoomService _roomService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateRoom(
            [FromRoute] int hotelId,
            [FromForm] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _roomService.CreateRoomAsync(hotelId, request);
            return Ok(room);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsByHotel(
            [FromRoute] int hotelId,
            [FromQuery] GetHotelRoomsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roomService.GetFilteredRoomsByHotelIdAsync(hotelId, request);
            return Ok(result);
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomById(
            [FromRoute] int hotelId,
            [FromRoute] int roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(hotelId, roomId);
            return Ok(room);
        }

        [HttpPut("{roomId}")]
        public async Task<IActionResult> UpdateRoom(
            [FromRoute] int hotelId,
            [FromRoute] int roomId,
            [FromForm] UpdateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _roomService.UpdateRoomAsync(hotelId, roomId, request);
            return Ok(updated);
        }

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
