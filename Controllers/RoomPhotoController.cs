using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Filters;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
    [EnsureAgencyExistsForOwner]
    [EnsureHotelExistsForOwner]
    [Route("api/hotels/{hotelId}/rooms/{roomId}/photos")]
    public class RoomPhotoController(IRoomPhotoService _roomPhotoService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadPhotos(
            [FromRoute] int hotelId,
            [FromRoute] int roomId,
            [FromForm] UploadRoomPhotosRequest request
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var photo = await _roomPhotoService.UploadPhotosAsync(roomId, request);
            return Ok(photo);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos(
            [FromRoute] int hotelId,
            [FromRoute] int roomId
        )
        {
            var photos = await _roomPhotoService.GetPhotosByRoomIdAsync(roomId);
            return Ok(photos);
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(
            [FromRoute] int hotelId,
            [FromRoute] int roomId,
            [FromRoute] int photoId
        )
        {
            await _roomPhotoService.DeletePhotoAsync(roomId, photoId);
            return NoContent();
        }
    }
}
