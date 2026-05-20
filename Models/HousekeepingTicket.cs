using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    public class HousekeepingTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Todo;

        [Required]
        public TicketType Type { get; set; }

        [Required]
        public TicketLocationType LocationType { get; set; }

        public int? RoomId { get; set; }

        public int? FacilityId { get; set; }

        public int AssignedToId { get; set; }

        [Required]
        public int CreatedById { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        [ForeignKey(nameof(FacilityId))]
        public Facility? Facility { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public ApplicationUser? AssignedTo { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public ApplicationUser? CreatedBy { get; set; }

        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}
