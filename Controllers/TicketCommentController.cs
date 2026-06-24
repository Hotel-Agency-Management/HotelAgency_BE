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
    [Route("api/hotels/{hotelId}/housekeeping-tickets/{ticketId}/comments")]
    [Authorize(Roles = $"{Roles.AgencyOwner},{Roles.PropertyManager}," +
                       $"{Roles.HousekeepingManager},{Roles.HousekeepingEmployee}," +
                       $"{Roles.FrontDeskStaff}," +
                       $"{Roles.CustomerSupport}")]
    public class TicketCommentController(
        ITicketCommentService _commentService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> GetComments(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId,
            [FromQuery] TicketCommentListRequest request)
        {
            var result = await _commentService.GetCommentsByTicketAsync(ticketId, request);
            return Ok(result);
        }

        [HttpPost]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> AddComment(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId,
            [FromBody] CreateTicketCommentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(Messages.Unauthorized);

            var result = await _commentService.AddCommentAsync(ticketId, user.Id, request);
            return Created(string.Empty, result);
        }

        [HttpPut("{commentId:int}")]
        [EnsureTicketBelongsToHotelAttribute]
        [EnsureCommentBelongsToTicketAttribute]
        public async Task<IActionResult> UpdateComment(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId,
            [FromRoute] int commentId,
            [FromBody] UpdateTicketCommentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(Messages.Unauthorized);

            var result = await _commentService.UpdateCommentAsync(ticketId, commentId, user.Id, request);
            return Ok(result);
        }

        [HttpDelete("{commentId:int}")]
        [EnsureTicketBelongsToHotelAttribute]
        [EnsureCommentBelongsToTicketAttribute]
        public async Task<IActionResult> DeleteComment(
            [FromRoute] int hotelId,
            [FromRoute] int ticketId,
            [FromRoute] int commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(Messages.Unauthorized);

            await _commentService.DeleteCommentAsync(ticketId, commentId, user.Id, hotelId: hotelId);
            return NoContent();
        }
    }
}
