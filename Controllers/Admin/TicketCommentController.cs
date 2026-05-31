using Booking.Constants;
using Booking.DTO;
using Booking.Filters;
using Booking.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnsureAgencyExistsForAdminAttribute]
    [EnsureHotelExistsForAdminAttribute]
    [Route("api/admin/agencies/{agencyId}/hotels/{hotelId}/housekeeping-tickets/{ticketId}/comments")]
    public class TicketCommentController(
        ITicketCommentService _commentService) : ControllerBase
    {
        [HttpGet]
        [EnsureTicketBelongsToHotelAttribute]
        public async Task<IActionResult> GetComments(
            [FromRoute] int ticketId,
            [FromQuery] TicketCommentListRequest request)
        {
            var result = await _commentService.GetCommentsByTicketAsync(ticketId, request);
            return Ok(result);
        }

        [HttpDelete("{commentId:int}")]
        [EnsureTicketBelongsToHotelAttribute]
        [EnsureCommentBelongsToTicketAttribute]
        public async Task<IActionResult> DeleteComment(
            [FromRoute] int ticketId,
            [FromRoute] int commentId)
        {
            await _commentService.DeleteCommentAsync(ticketId, commentId, requestingUserId: 0, isAdmin: true);
            return NoContent();
        }
    }
}
