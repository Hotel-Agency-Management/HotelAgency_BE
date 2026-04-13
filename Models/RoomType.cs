using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Constants;

namespace Booking.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public decimal DailyPrice { get; set; }

        public decimal WeeklyPrice { get; set; }

        public decimal MonthlyPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }

        //public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}