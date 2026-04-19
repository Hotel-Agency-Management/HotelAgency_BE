using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId}/hotels/{hotelId}/rooms/{roomId}/photos")]
    public class RoomPhotoController(IRoomPhotoService _roomPhotoService) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner},{Roles.PropertyManager}")]
        [HttpPost]
        public async Task<IActionResult> UploadPhotos(
            [FromRoute] int roomId,
            [FromForm] UploadRoomPhotosRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var photo = await _roomPhotoService.UploadPhotosAsync(roomId, request);
            return Ok(photo);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner},{Roles.PropertyManager}")]
        [HttpGet]
        public async Task<IActionResult> GetPhotos([FromRoute] int roomId)
        {
            var photos = await _roomPhotoService.GetPhotosByRoomIdAsync(roomId);
            return Ok(photos);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner},{Roles.PropertyManager}")]
        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(
            [FromRoute] int roomId,
            [FromRoute] int photoId)
        {
            await _roomPhotoService.DeletePhotoAsync(roomId, photoId);
            return NoContent();
        }
    }
}
