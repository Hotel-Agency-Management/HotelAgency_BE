using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [Route("api/admin/room-amenities")]
    public class RoomAmenityAdminController(IRoomAmenityService _amenityService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAmenity([FromBody] CreateRoomAmenityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var amenity = await _amenityService.CreateAmenityAsync(request);
            return Ok(amenity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAmenities()
        {
            var amenities = await _amenityService.GetAllAmenitiesAsync();
            return Ok(amenities);
        }

        [HttpGet("{amenityId}")]
        public async Task<IActionResult> GetAmenityById([FromRoute] int amenityId)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(amenityId);
            return Ok(amenity);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin}")]
        [HttpDelete("{amenityId}")]
        public async Task<IActionResult> DeleteAmenity([FromRoute] int amenityId)
        {
            await _amenityService.DeleteAmenityAsync(amenityId);
            return NoContent();
        }
    }
}
