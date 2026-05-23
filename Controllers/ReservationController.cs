using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Booking.Constants;
using Booking.DTO;
using Booking.Enums;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Booking.Models;


namespace Booking.Controllers
{
    [ApiController]
    [EnsureAgencyExistsForOwner]
    [EnsureHotelExistsForOwner]
    [Route("api/hotels/{hotelId}/reservations")]
    public class ReservationController(
        IReservationService _reservationService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.FrontDeskStaff}, {Roles.PropertyManager}, {Roles.Customer}")]
        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            [FromRoute] int hotelId,
            [FromForm] CreateReservationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(Messages.Unauthorized);

            var result = await _reservationService.CreateReservationAsync(hotelId, user.Id, request);
            return CreatedAtAction(nameof(GetReservationById), new { hotelId, reservationId = result.Id }, result);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.FrontDeskStaff}, {Roles.PropertyManager}")]
        [HttpGet]
        public async Task<IActionResult> GetReservationsByHotel(
            [FromRoute] int hotelId,
            [FromQuery] ReservationListRequest request)
        {
            var result = await _reservationService.GetReservationsByHotelIdAsync(hotelId, request);
            return Ok(result);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.FrontDeskStaff},{Roles.PropertyManager}")]
        [HttpGet("{reservationId}")]
        public async Task<IActionResult> GetReservationById(
            [FromRoute] int hotelId,
            [FromRoute] int reservationId)
        {
            var result = await _reservationService.GetReservationByIdAsync(hotelId, reservationId);
            return Ok(result);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.FrontDeskStaff}, {Roles.PropertyManager}")]
        [HttpPut("{reservationId}")]
        public async Task<IActionResult> UpdateReservation(
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

        [Authorize(Roles = $"{Roles.AgencyOwner}, {Roles.FrontDeskStaff}, {Roles.PropertyManager}, {Roles.Customer}")]
        [HttpPatch("{reservationId}/cancel")]
        public async Task<IActionResult> CancelReservation(
            [FromRoute] int hotelId,
            [FromRoute] int reservationId,
            [FromBody] CancelReservationRequest request)
        {
            var result = await _reservationService.CancelReservationAsync(hotelId, reservationId, request);
            return Ok(result);
        }

        [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.FrontDeskStaff},{Roles.PropertyManager}")]
        [HttpPatch("{reservationId}/status")]
        public async Task<IActionResult> UpdateReservationStatus(
            [FromRoute] int hotelId,
            [FromRoute] int reservationId,
            [FromBody] UpdateReservationStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reservationService.UpdateReservationStatusAsync(hotelId, reservationId, request);
            return Ok(result);
        }
    }
}
