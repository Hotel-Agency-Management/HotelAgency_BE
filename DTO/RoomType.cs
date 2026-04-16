using System.ComponentModel.DataAnnotations;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateRoomTypeRequest
    {
        [Required] public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public decimal DailyPrice { get; set; }
        [Required] public decimal WeeklyPrice { get; set; }
        [Required] public decimal MonthlyPrice { get; set; }
    }

    public class UpdateRoomTypeRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public decimal? DailyPrice { get; set; }
        public decimal? WeeklyPrice { get; set; }
        public decimal? MonthlyPrice { get; set; }
    }

    public class RoomTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public decimal DailyPrice { get; set; }
        public decimal WeeklyPrice { get; set; }
        public decimal MonthlyPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public RoomTypeResponse(RoomType roomType)
        {
            Id = roomType.Id;
            Name = roomType.Name;
            Description = roomType.Description;
            Capacity = roomType.Capacity;
            DailyPrice = roomType.DailyPrice;
            WeeklyPrice = roomType.WeeklyPrice;
            MonthlyPrice = roomType.MonthlyPrice;
            CreatedAt = roomType.CreatedAt;
            UpdatedAt = roomType.UpdatedAt;
        }
    }
}
