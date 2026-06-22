using Booking.Enums;

namespace Booking.DTO
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public NotificationResponse(Models.Notification n)
        {
            Id = n.Id;
            UserId = n.UserId;
            Title = n.Title;
            Message = n.Message;
            Type = n.Type.ToString();
            Metadata = n.Metadata;
            IsRead = n.IsRead;
            ReadAt = n.ReadAt;
            CreatedAt = n.CreatedAt;
        }
    }

    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; } = NotificationType.General;
        public string? Metadata { get; set; }
    }

    public class NotificationListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool? IsRead { get; set; }
        public NotificationType? Type { get; set; }
    }
}
