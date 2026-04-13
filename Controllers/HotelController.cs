using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;
using Microsoft.AspNetCore.Authorization;


namespace Booking.Controllers
{
    [ApiController]
    [Route("api/agencies/{agencyId}/hotels")]
    public class HotelController(IHotelService _hotelService) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpPost]
        public async Task<IActionResult> CreateHotel(
            [FromRoute] int agencyId,
            [FromForm] CreateHotelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.AgencyId = agencyId;

            var hotel = await _hotelService.CreateHotelAsync(request);
            return Ok(hotel);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpGet]
        public async Task<IActionResult> GetHotelsByAgency([FromRoute] int agencyId)
        {
            var hotels = await _hotelService.GetHotelsByAgencyIdAsync(agencyId);
            return Ok(hotels);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}, {Roles.PropertyManager}")]
        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelById([FromRoute] int hotelId)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            return Ok(hotel);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpPut("{hotelId}")]
        public async Task<IActionResult> UpdateHotel(
            [FromRoute] int hotelId,
            [FromForm] UpdateHotelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _hotelService.UpdateHotelAsync(hotelId, request);
            return Ok(updated);
        }

        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AgencyOwner}")]
        [HttpDelete("{hotelId}")]
        public async Task<IActionResult> DeleteHotel([FromRoute] int hotelId)
        {
            await _hotelService.DeleteHotelAsync(hotelId);
            return NoContent();
        }
    }
}
