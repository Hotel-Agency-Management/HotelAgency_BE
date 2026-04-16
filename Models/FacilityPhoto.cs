using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models
{
    public class FacilityPhoto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(FacilityId))]
        public int FacilityId { get; set; }

        [Required]
        public string PhotoUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Facility? Facility { get; set; }
    }
}
