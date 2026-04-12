using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    public class Facility
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string FacilityType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public FacilityStatus Status { get; set; }

        public TimeOnly? OpenAt { get; set; }

        public TimeOnly? CloseAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Hotel? Hotel { get; set; }
    }
}
