using Booking.Constants;
using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Hubs;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Booking.Services
{
    public class NotificationService(
        INotificationRepository _notificationRepository,
        IHubContext<NotificationHub> _hubContext,
        UserManager<ApplicationUser> _userManager,
        ILogger<NotificationService> _logger) : INotificationService
    {
        public async Task<NotificationResponse> CreateAsync(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _notificationRepository.CreateAsync(notification);
            _logger.LogInformation("Notification {NotificationId} created for user {UserId}", saved.Id, saved.UserId);

            var response = new NotificationResponse(saved);
            await _hubContext.Clients.Group(saved.UserId.ToString())
                .SendAsync("ReceiveNotification", response);

            return response;
        }

        public async Task<NotificationListResponse> GetUserNotificationsAsync(
            int userId, NotificationListRequest request)
        {
            var (items, totalCount) = await _notificationRepository.GetByUserIdAsync(
                userId, request.IsRead, request.Type, request.PageNumber, request.PageSize);

            var unreadCount = await _notificationRepository.CountUnreadByUserIdAsync(userId);

            return new NotificationListResponse
            {
                Items = items.Select(n => new NotificationResponse(n)).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                UnreadCount = unreadCount
            };
        }

        public async Task MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAndUserIdAsync(notificationId, userId)
                ?? throw new NotificationNotFoundException(notificationId);

            if (notification.IsRead)
                return;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _notificationRepository.UpdateAsync(notification);
            _logger.LogInformation("Notification {NotificationId} marked as read by user {UserId}", notificationId, userId);
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllAsReadByUserIdAsync(userId);
            _logger.LogInformation("All notifications marked as read for user {UserId}", userId);
        }

        public async Task NotifySuperAdminsAsync(string title, string message, NotificationType type, string? metadata = null)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync(Roles.SuperAdmin);
            foreach (var admin in superAdmins)
            {
                await CreateAsync(new CreateNotificationRequest
                {
                    UserId = admin.Id,
                    Title = title,
                    Message = message,
                    Type = type,
                    Metadata = metadata
                });
            }
        }
    }
}
