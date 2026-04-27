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
    [EnsureFacilityBelongsToHotel]
    [Route("api/admin/agencies/{agencyId}/hotels/{hotelId}/facilities/{facilityId}/photos")]
    public class FacilityPhotoController(IFacilityPhotoService _photoService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadPhotos(
            [FromRoute] int facilityId,
            [FromForm] UploadPhotosRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var photo = await _photoService.UploadPhotosAsync(facilityId, request);
            return Ok(photo);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos([FromRoute] int facilityId)
        {
            var photos = await _photoService.GetPhotosByFacilityIdAsync(facilityId);
            return Ok(photos);
        }

        [HttpGet("{photoId}")]
        public async Task<IActionResult> GetPhoto([FromRoute] int photoId)
        {
            var photo = await _photoService.GetPhotoByIdAsync(photoId);
            return Ok(photo);
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(
            [FromRoute] int photoId)
        {
            await _photoService.DeletePhotoAsync(photoId);
            return NoContent();
        }
    }
}
