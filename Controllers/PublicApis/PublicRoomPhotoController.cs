using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Filters;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/public/hotels/{hotelId}/rooms/{roomId}/photos")]
    public class PublicRoomPhotoController(IRoomPhotoService _roomPhotoService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPhotos(
            [FromRoute] int hotelId,
            [FromRoute] int roomId
        )
        {
            var photos = await _roomPhotoService.GetPhotosByRoomIdAsync(roomId);
            return Ok(photos);
        }
    }
}
