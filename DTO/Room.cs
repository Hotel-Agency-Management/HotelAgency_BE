using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateRoomRequest
    {
        [Required] public int RoomTypeId { get; set; }
        [Required] public string RoomNumber { get; set; } = string.Empty;
        [Required][Range(0, int.MaxValue)] public int FloorNumber { get; set; }
        public string? Description { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        public string? Notes { get; set; }
        public IFormFile coverPhoto { get; set; } = null!;
        public List<int> AmenityIds { get; set; } = new();
        [Required] public decimal DailyPrice { get; set; }
        [Required] public decimal WeeklyPrice { get; set; }
        [Required] public decimal MonthlyPrice { get; set; }
        [Required] public decimal ExtendPrice { get; set; }
        [Required] public int Capacity { get; set; }

    }

    public class UpdateRoomRequest
    {
        public int? RoomTypeId { get; set; }
        public string? RoomNumber { get; set; }
        public int? FloorNumber { get; set; }
        public string? Description { get; set; }
        public RoomStatus? Status { get; set; }
        public string? Notes { get; set; }
        public IFormFile? CoverPhoto { get; set; }
        public decimal? DailyPrice { get; set; }
        public decimal? WeeklyPrice { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public decimal? ExtendPrice { get; set; }
        public int? Capacity { get; set; }

    }

    public class RoomResponse
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal DailyPrice { get; set; }
        public decimal WeeklyPrice { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal ExtendPrice { get; set; }
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CoverPhotoUrl { get; set; }
        public List<RoomAmenityResponse> Amenities { get; set; } = new();

        public RoomResponse(Room room)
        {
            Id = room.Id;
            HotelId = room.HotelId;
            RoomTypeId = room.RoomTypeId;
            RoomNumber = room.RoomNumber;
            FloorNumber = room.FloorNumber;
            Description = room.Description;
            Status = room.Status.ToString();
            Notes = room.Notes;
            DailyPrice = room.DailyPrice;
            MonthlyPrice = room.MonthlyPrice;
            WeeklyPrice = room.WeeklyPrice;
            ExtendPrice = room.ExtendPrice;
            Capacity = room.Capacity;
            CreatedAt = room.CreatedAt;
            UpdatedAt = room.UpdatedAt;
            CoverPhotoUrl = room.CoverPhotoUrl ?? string.Empty;
            RoomTypeName = room.RoomType!.Name;
            Amenities = room.Amenities
            .Select(a => new RoomAmenityResponse(a))
            .ToList();
        }
    }

    public class ListRoomResponse
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public string Status { get; set; } = string.Empty;

        public ListRoomResponse(Room room)
        {
            Id = room.Id;
            HotelId = room.HotelId;
            RoomNumber = room.RoomNumber;
            FloorNumber = room.FloorNumber;
            Status = room.Status.ToString();
            RoomTypeName = room.RoomType!.Name;
        }
    }
}
