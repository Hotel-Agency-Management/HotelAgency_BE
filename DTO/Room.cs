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
        [Range(0, double.MaxValue, ErrorMessage = "Insurance value cannot be negative.")]
        public decimal? InsurancePerReservation { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Insurance value cannot be negative.")]
        public decimal? YearlyInsurance { get; set; }

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
        [Range(0, double.MaxValue, ErrorMessage = "Insurance value cannot be negative.")]
        public decimal? InsurancePerReservation { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Insurance value cannot be negative.")]
        public decimal? YearlyInsurance { get; set; }

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
        public decimal? InsurancePerReservation { get; set; }
        public decimal? YearlyInsurance { get; set; }
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
            InsurancePerReservation = room.InsurancePerReservation;
            YearlyInsurance = room.YearlyInsurance;
            CreatedAt = room.CreatedAt;
            UpdatedAt = room.UpdatedAt;
            CoverPhotoUrl = room.CoverPhotoUrl ?? string.Empty;
            RoomTypeName = room.RoomType!.Name;
            Amenities = room.Amenities
            .Select(a => new RoomAmenityResponse(a))
            .ToList();
        }
    }

    public class GetHotelRoomsRequest
    {
        [Range(1, int.MaxValue)] public int PageNumber { get; set; } = 1;
        [Range(1, 100)] public int PageSize { get; set; } = 10;
        public DateOnly? CheckIn { get; set; }
        public DateOnly? CheckOut { get; set; }
        public int? Guests { get; set; }
        public int? Rooms { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchText { get; set; }
        public int? RoomTypeId { get; set; }
    }

    public class HotelRoomResponse
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? InsurancePerReservation { get; set; }
        public List<string> Amenities { get; set; } = [];
        public string? MainPhotoUrl { get; set; }

        public HotelRoomResponse(Room room)
        {
            RoomId = room.Id;
            RoomNumber = room.RoomNumber;
            RoomType = room.RoomType!.Name;
            PricePerNight = room.DailyPrice;
            Capacity = room.Capacity;
            Status = room.Status.ToString();
            Description = room.Description;
            InsurancePerReservation = room.InsurancePerReservation;
            Amenities = room.Amenities.Select(a => a.Name).ToList();
            MainPhotoUrl = room.CoverPhotoUrl;
        }
    }

    public class RoomStatusDistributionItem
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RoomStatusDistributionResponse
    {
        public int TotalRooms { get; set; }
        public List<RoomStatusDistributionItem> Items { get; set; } = new();
    }
}
