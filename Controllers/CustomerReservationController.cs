using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Route("api/my-reservations")]
    [Authorize(Roles = Roles.Customer)]
    public class CustomerReservationController(
        IReservationService _reservationService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateMyReservation([FromBody] CustomerCreateReservationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);
            var result = await _reservationService.CreateMyReservationAsync(
                user.Id,
                $"{user.FirstName} {user.LastName}".Trim(),
                user.Email!,
                user.PhoneNumber ?? string.Empty,
                request);
            return CreatedAtAction(nameof(GetMyReservationById), new { reservationId = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyReservations([FromQuery] ReservationListRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);
            var result = await _reservationService.GetMyReservationsAsync(user.Id, request);
            return Ok(result);
        }

        [HttpGet("{reservationId}")]
        public async Task<IActionResult> GetMyReservationById([FromRoute] int reservationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);
            var result = await _reservationService.GetMyReservationByIdAsync(reservationId, user.Id);
            return Ok(result);
        }

        [HttpPut("{reservationId}")]
        public async Task<IActionResult> UpdateMyReservation(
            [FromRoute] int reservationId,
            [FromBody] UpdateReservationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);
            var result = await _reservationService.UpdateMyReservationAsync(reservationId, user.Id, request);
            return Ok(result);
        }

        [HttpPatch("{reservationId}/cancel")]
        public async Task<IActionResult> CancelMyReservation(
            [FromRoute] int reservationId,
            [FromBody] CancelReservationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(Messages.Unauthorized);
            var result = await _reservationService.CancelMyReservationAsync(reservationId, user.Id, request);
            return Ok(result);
        }
    }
}
