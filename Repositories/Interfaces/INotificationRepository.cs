using Booking.Enums;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification?> GetByIdAsync(int id);
        Task<Notification?> GetByIdAndUserIdAsync(int id, int userId);
        Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetByUserIdAsync(
            int userId, bool? isRead, NotificationType? type, int pageNumber, int pageSize);
        Task<int> CountUnreadByUserIdAsync(int userId);
        Task UpdateAsync(Notification notification);
        Task MarkAllAsReadByUserIdAsync(int userId);
    }
}
