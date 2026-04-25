using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;
using Microsoft.AspNetCore.Authorization;
using Booking.Filters;

namespace Booking.Controllers.Admin
{

    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [EnsureAgencyExistsForAdminAttribute]
    [EnsureHotelExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId}/hotels/{hotelId}/facilities")]
    public class FacilityController(IFacilityService _facilityService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateFacility(
            [FromRoute] int hotelId,
            [FromBody] CreateFacilityRequest request
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var facility = await _facilityService.CreateFacilityAsync(hotelId, request);
            return Created(string.Empty, facility);
        }

        [HttpGet]
        public async Task<IActionResult> GetFacilitiesByHotel(
            [FromRoute] int hotelId
        )
        {
            var facilities = await _facilityService.GetFacilitiesByHotelIdAsync(hotelId);
            return Ok(facilities);
        }

        [HttpGet("{facilityId}")]
        public async Task<IActionResult> GetFacilityById(
            [FromRoute] int facilityId)
        {
            var facility = await _facilityService.GetFacilityByIdAsync(facilityId);
            return Ok(facility);
        }

        [HttpPut("{facilityId}")]
        public async Task<IActionResult> UpdateFacility(
            [FromRoute] int facilityId,
            [FromBody] UpdateFacilityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _facilityService.UpdateFacilityAsync(facilityId, request);
            return Ok(updated);
        }

        [HttpDelete("{facilityId}")]
        public async Task<IActionResult> DeleteFacility(
            [FromRoute] int facilityId)
        {
            await _facilityService.DeleteFacilityAsync(facilityId);
            return NoContent();
        }
    }
}
