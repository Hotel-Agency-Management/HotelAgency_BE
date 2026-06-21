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

        public int? From { get; set; }
        public int? To { get; set; }

        public int? HotelId { get; set; }
        public int? AgencyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReservationId))] public Reservation? Reservation { get; set; }
        [ForeignKey(nameof(HotelId))] public Hotel? Hotel { get; set; }
        [ForeignKey(nameof(AgencyId))] public Agency? Agency { get; set; }
    }
}
