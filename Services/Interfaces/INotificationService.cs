using Booking.DTO;
using Booking.Enums;

namespace Booking.Interfaces.Services
{
    public interface INotificationService
    {
        Task<NotificationResponse> CreateAsync(CreateNotificationRequest request);
        Task<NotificationListResponse> GetUserNotificationsAsync(int userId, NotificationListRequest request);
        Task MarkAsReadAsync(int userId, int notificationId);
        Task MarkAllAsReadAsync(int userId);
        Task NotifySuperAdminsAsync(string title, string message, NotificationType type, string? metadata = null);
    }
}
