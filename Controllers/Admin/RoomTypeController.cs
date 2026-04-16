using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [Route("api/admin/room-types")]
    public class RoomTypeController(IRoomTypeService _roomTypeService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateRoomType(
            [FromRoute] int hotelId,
            [FromBody] CreateRoomTypeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomType = await _roomTypeService.CreateRoomTypeAsync(hotelId, request);
            return Ok(roomType);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomTypesByHotel([FromRoute] int hotelId)
        {
            var roomTypes = await _roomTypeService.GetRoomTypesByHotelIdAsync(hotelId);
            return Ok(roomTypes);
        }

        [HttpGet("{roomTypeId}")]
        public async Task<IActionResult> GetRoomTypeById(
            [FromRoute] int hotelId,
            [FromRoute] int roomTypeId)
        {
            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(hotelId, roomTypeId);
            return Ok(roomType);
        }

        [HttpPut("{roomTypeId}")]
        public async Task<IActionResult> UpdateRoomType(
            [FromRoute] int hotelId,
            [FromRoute] int roomTypeId,
            [FromBody] UpdateRoomTypeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _roomTypeService.UpdateRoomTypeAsync(hotelId, roomTypeId, request);
            return Ok(updated);
        }

        [HttpDelete("{roomTypeId}")]
        public async Task<IActionResult> DeleteRoomType(
            [FromRoute] int hotelId,
            [FromRoute] int roomTypeId)
        {
            await _roomTypeService.DeleteRoomTypeAsync(hotelId, roomTypeId);
            return NoContent();
        }
    }
}
