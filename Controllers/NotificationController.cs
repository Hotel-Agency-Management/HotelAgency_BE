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
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController(
        INotificationService _notificationService,
        UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationListRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized(Messages.Unauthorized);

            var result = await _notificationService.GetUserNotificationsAsync(user.Id, request);
            return Ok(result);
        }

        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized(Messages.Unauthorized);

            await _notificationService.MarkAsReadAsync(user.Id, id);
            return NoContent();
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized(Messages.Unauthorized);

            await _notificationService.MarkAllAsReadAsync(user.Id);
            return NoContent();
        }
    }
}
