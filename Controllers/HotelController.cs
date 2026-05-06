using Booking.DTO;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Booking.Constants;
using Microsoft.AspNetCore.Authorization;
using Booking.Filters;
using Booking.Models;
using Microsoft.AspNetCore.Identity;


namespace Booking.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{Roles.AgencyOwner}")]
    [EnsureAgencyExistsForOwner]
    [Route("api/hotels")]
    public class HotelController(
        IHotelService _hotelService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.AgencyOwner}")]
        [HttpPost]
        public async Task<IActionResult> CreateHotel(
            [FromForm] CreateHotelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var agencyOwner = await _userManager.GetUserAsync(User);
            request.AgencyId = agencyOwner!.AgencyId!.Value;

            var hotel = await _hotelService.CreateHotelAsync(request);
            return Ok(hotel);
        }


        [HttpGet]
        public async Task<IActionResult> GetHotelsByAgency([FromQuery] HotelListRequest request)
        {
            var agencyOwner = await _userManager.GetUserAsync(User);
            var result = await _hotelService.GetHotelsByAgencyIdAsync(agencyOwner!.AgencyId!.Value, request);
            return Ok(result);
        }

        [EnsureHotelExistsForOwnerAttribute]
        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelById([FromRoute] int hotelId)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            return Ok(hotel);
        }

        [EnsureHotelExistsForOwnerAttribute]
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
    }
}
