using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Booking.Models;
using Microsoft.AspNetCore.Identity;


namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    [EnsureAgencyExistsForAdminAttribute]
    [EnsureHotelExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId}/hotels/{hotelId}/reservations")]
    public class ReservationController(
        IReservationService _reservationService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromForm] CreateReservationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(Messages.Unauthorized);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reservationService.CreateReservationAsync(hotelId, user.Id, request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservationsByHotel(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId)
        {
            var result = await _reservationService.GetReservationsByHotelIdAsync(hotelId);
            return Ok(result);
        }

        [HttpGet("{reservationId}")]
        public async Task<IActionResult> GetReservationById(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromRoute] int reservationId)
        {
            var result = await _reservationService.GetReservationByIdAsync(hotelId, reservationId);
            return Ok(result);
        }

        [HttpPut("{reservationId}")]
        public async Task<IActionResult> UpdateReservation(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromRoute] int reservationId,
            [FromBody] UpdateReservationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(Messages.Unauthorized);

            var result = await _reservationService.UpdateReservationAsync(hotelId, reservationId, user.Id, request);
            return Ok(result);
        }

        [HttpPatch("{reservationId}/status")]
        public async Task<IActionResult> UpdateStatus(
            [FromRoute] int agencyId,
            [FromRoute] int hotelId,
            [FromRoute] int reservationId,
            [FromBody] UpdateReservationStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reservationService.UpdateReservationStatusAsync(hotelId, reservationId, request.Status);
            return Ok(result);
        }
    }
}
