using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;
using Microsoft.AspNetCore.Authorization;
using Booking.Filters;

namespace Booking.Controllers
{

    [ApiController]
    [EnsureAgencyExistsForOwner]
    [EnsureHotelExistsForOwnerAttribute]
    [Route("api/hotels/{hotelId}/facilities")]
    public class FacilityController(IFacilityService _facilityService) : ControllerBase
    {

        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
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

        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.PropertyManager}, {Roles.HousekeepingManager}, {Roles.FrontDeskStaff}")]
        [HttpGet]
        public async Task<IActionResult> GetFacilitiesByHotel([FromRoute] int hotelId)
        {
            var facilities = await _facilityService.GetFacilitiesByHotelIdAsync(hotelId);
            return Ok(facilities);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
        [HttpGet("{facilityId}")]
        [EnsureFacilityBelongsToHotel]
        public async Task<IActionResult> GetFacilityById(
                [FromRoute] int hotelId,
                [FromRoute] int facilityId)
        {
            var facility = await _facilityService.GetFacilityByIdAsync(facilityId);
            return Ok(facility);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
        [HttpPut("{facilityId}")]
        [EnsureFacilityBelongsToHotel]
        public async Task<IActionResult> UpdateFacility(
            [FromRoute] int hotelId,
            [FromRoute] int facilityId,
            [FromBody] UpdateFacilityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _facilityService.UpdateFacilityAsync(facilityId, request);
            return Ok(updated);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}")]
        [HttpDelete("{facilityId}")]
        [EnsureFacilityBelongsToHotel]
        public async Task<IActionResult> DeleteFacility(
            [FromRoute] int hotelId,
            [FromRoute] int facilityId)
        {
            await _facilityService.DeleteFacilityAsync(facilityId);
            return NoContent();
        }
    }
}
