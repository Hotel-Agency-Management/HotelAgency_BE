using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.PropertyManager}")]
    [Route("api/room-amenities")]
    public class RoomAmenityController(IRoomAmenityService _amenityService) : ControllerBase
    {
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
    }
}
