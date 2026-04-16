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
            [FromBody] CreateRoomTypeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomType = await _roomTypeService.CreateRoomTypeAsync(request);
            return Ok(roomType);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomTypes()
        {
            var roomTypes = await _roomTypeService.GetRoomTypesAsync();
            return Ok(roomTypes);
        }

        [HttpGet("{roomTypeId}")]
        public async Task<IActionResult> GetRoomTypeById(
            [FromRoute] int roomTypeId)
        {
            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(roomTypeId);
            return Ok(roomType);
        }

        [HttpPut("{roomTypeId}")]
        public async Task<IActionResult> UpdateRoomType(
            [FromRoute] int roomTypeId,
            [FromBody] UpdateRoomTypeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _roomTypeService.UpdateRoomTypeAsync(roomTypeId, request);
            return Ok(updated);
        }

        [HttpDelete("{roomTypeId}")]
        public async Task<IActionResult> DeleteRoomType(
            [FromRoute] int roomTypeId)
        {
            await _roomTypeService.DeleteRoomTypeAsync(roomTypeId);
            return NoContent();
        }
    }
}
