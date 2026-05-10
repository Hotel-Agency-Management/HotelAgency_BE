using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Constants;
using Booking.Enums;

namespace Booking.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        public int FloorNumber { get; set; }

        public string? Description { get; set; }

        public RoomStatus Status { get; set; } = RoomStatus.Available;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(HotelId))]
        public Hotel? Hotel { get; set; }

        [ForeignKey(nameof(RoomTypeId))]
        public RoomType? RoomType { get; set; }
        public string? CoverPhotoUrl { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public decimal DailyPrice { get; set; }
        [Required] public decimal WeeklyPrice { get; set; }
        [Required] public decimal MonthlyPrice { get; set; }
        [Required] public decimal ExtendPrice { get; set; }
        public decimal? MonthlyInsurance { get; set; }
        public ICollection<RoomPhoto> Photos { get; set; } = new List<RoomPhoto>();
        public ICollection<RoomAmenity> Amenities { get; set; } = new List<RoomAmenity>();

    }
}
