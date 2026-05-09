using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booking.Filters;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/public/hotels/{hotelId}/rooms")]
    public class PublicRoomController(IRoomService _roomService) : ControllerBase
    {
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
    }
}
