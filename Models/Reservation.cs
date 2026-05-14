using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    public class Reservation
    {
        [Key] public int Id { get; set; }
        [Required] public string ReservationNumber { get; set; } = string.Empty;

        [Required] public int HotelId { get; set; }
        public int? CustomerId { get; set; }

        [Required] public ReservationSource Source { get; set; } = ReservationSource.WalkIn;
        [Required] public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        [Required] public string GuestFullName { get; set; } = string.Empty;
        [Required] public string GuestEmail { get; set; } = string.Empty;
        [Required] public string GuestPhone { get; set; } = string.Empty;
        public string GuestIdNumber { get; set; } = string.Empty;

        [Required] public DateOnly CheckInDate { get; set; }
        [Required] public DateOnly CheckOutDate { get; set; }
        [Required] public int NumberOfGuests { get; set; }
        [Required] public int NumberOfRooms { get; set; }
        [Required] public Decimal TotalAmount { get; set; }

        public string? ContractPath { get; set; }
        public string? InvoicePath { get; set; }

        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }

        public DateTime? CancelledAt { get; set; }
        public decimal CancellationFee { get; set; } = 0;
        public bool IsFreeCancellation { get; set; } = false;
        public bool HasInsurance { get; set; } = false;
        public decimal InsuranceAmount { get; set; } = 0m;
        public string? CancellationReason { get; set; }

        [Required] public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(HotelId))] public Hotel? Hotel { get; set; }
        [ForeignKey(nameof(CustomerId))] public ApplicationUser? Customer { get; set; }
        [ForeignKey(nameof(CreatedById))] public ApplicationUser? CreatedBy { get; set; }
        [ForeignKey(nameof(UpdatedById))] public ApplicationUser? UpdatedBy { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; } = [];
        public ICollection<PaymentLog> PaymentLogs { get; set; } = [];
    }
}
