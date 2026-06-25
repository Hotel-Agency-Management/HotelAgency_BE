using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class NotificationRepository(ApplicationDbContext _context) : INotificationRepository
    {
        public async Task<Notification> CreateAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification?> GetByIdAsync(int id)
            => await _context.Notifications.FindAsync(id);

        public async Task<Notification?> GetByIdAndUserIdAsync(int id, int userId)
            => await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetByUserIdAsync(
            int userId, bool? isRead, NotificationType? type, int pageNumber, int pageSize)
        {
            var query = BuildQuery(userId, isRead, type);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task<int> CountUnreadByUserIdAsync(int userId)
            => await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadByUserIdAsync(int userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = now;
            }

            await _context.SaveChangesAsync();
        }

        private IQueryable<Notification> BuildQuery(int userId, bool? isRead, NotificationType? type)
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);

            if (isRead.HasValue)
                query = query.Where(n => n.IsRead == isRead.Value);

            if (type.HasValue)
                query = query.Where(n => n.Type == type.Value);

            return query;
        }
    }
}
