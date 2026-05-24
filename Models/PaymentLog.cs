using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    public class PaymentLog
    {
        [Key] public int Id { get; set; }

        public int? ReservationId { get; set; }
        [Required] public decimal Amount { get; set; }
        [Required] public PaymentType Type { get; set; }
        public string? Reason { get; set; }

        [Required] public int From { get; set; }
        [Required] public int To { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReservationId))] public Reservation? Reservation { get; set; }
    }
}
