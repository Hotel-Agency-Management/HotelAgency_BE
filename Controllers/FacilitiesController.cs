using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId}/hotels/{hotelId}/facilities")]
    public class FacilityController(IFacilityService _facilityService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateFacility(
            [FromRoute] int hotelId,
            [FromBody] CreateFacilityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var facility = await _facilityService.CreateFacilityAsync(hotelId, request);
            return Created(string.Empty, facility);
        }

        [HttpGet]
        public async Task<IActionResult> GetFacilitiesByHotel([FromRoute] int hotelId)
        {
            var facilities = await _facilityService.GetFacilitiesByHotelIdAsync(hotelId);
            return Ok(facilities);
        }

        public async Task<IActionResult> GetFacilityById(
            [FromRoute] int facilityId)
        {
            var facility = await _facilityService.GetFacilityByIdAsync(facilityId);
            return Ok(facility);
        }

        public async Task<IActionResult> UpdateFacility(
            [FromRoute] int facilityId,
            [FromBody] UpdateFacilityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _facilityService.UpdateFacilityAsync(facilityId, request);
            return Ok(updated);
        }

        public async Task<IActionResult> DeleteFacility(
            [FromRoute] int facilityId)
        {
            await _facilityService.DeleteFacilityAsync(facilityId);
            return NoContent();
        }
    }
}
