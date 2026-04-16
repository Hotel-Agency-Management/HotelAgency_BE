using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class RoomAmenity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
