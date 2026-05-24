using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [EnsureAgencyExistsForOwnerAttribute]
    [EnsureHotelExistsForOwnerAttribute]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}," +
                       $"{Roles.HousekeepingManager},{Roles.FrontDeskStaff}," +
                       $"{Roles.HousekeepingEmployee}")]
                       
    [Route("api/hotels/{hotelId}/housekeeping-tickets")]
    public class HousekeepingTicketController(
        IHousekeepingTicketService _ticketService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTicket(
            [FromRoute] int hotelId,
            [FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(Messages.Unauthorized);

            var result = await _ticketService.CreateTicketAsync(hotelId, user.Id, request);
            return Created(string.Empty, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets(
            [FromRoute] int hotelId,
            [FromQuery] TicketListRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(Messages.Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _ticketService.GetTicketsByHotelAsync(hotelId, user.Id, roles, request);
            return Ok(result);
        }

        [HttpGet("board")]
        public async Task<IActionResult> GetBoard([FromRoute] int hotelId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(Messages.Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _ticketService.GetBoardAsync(hotelId, user.Id, roles);
            return Ok(result);
        }

        [HttpGet("{ticketId:int}")]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> GetTicketById(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId)
        {
            var result = await _ticketService.GetTicketByIdAsync(hotelId, ticketId);
            return Ok(result);
        }

        [HttpPut("{ticketId:int}")]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> UpdateTicket(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId,
            [FromBody] UpdateTicketRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _ticketService.UpdateTicketAsync(hotelId, ticketId, request);
            return Ok(result);
        }

        [HttpPatch("{ticketId:int}/status")]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> UpdateTicketStatus(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId,
            [FromBody] UpdateTicketStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _ticketService.UpdateTicketStatusAsync(hotelId, ticketId, request);
            return Ok(result);
        }

        [HttpDelete("{ticketId:int}")]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> DeleteTicket(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId)
        {
            await _ticketService.DeleteTicketAsync(hotelId, ticketId);
            return NoContent();
        }
    }
}
