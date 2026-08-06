using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    public class Notification
    {
        [Key] public int Id { get; set; }
        public int UserId { get; set; }
        [Required][MaxLength(200)] public string Title { get; set; } = string.Empty;
        [Required] public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string? Metadata { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))] public ApplicationUser User { get; set; } = null!;
    }
}
