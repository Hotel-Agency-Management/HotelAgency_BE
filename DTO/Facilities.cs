using System.ComponentModel.DataAnnotations;
using Booking.Models;
using Booking.Enums;

namespace Booking.DTO
{
    public class CreateFacilityRequest
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string FacilityType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FacilityStatus Status { get; set; }
        public TimeOnly? OpenAt { get; set; }
        public TimeOnly? CloseAt { get; set; }
    }

    public class UpdateFacilityRequest
    {
        public string? Name { get; set; }
        public string? FacilityType { get; set; }
        public string? Description { get; set; }
        public FacilityStatus? Status { get; set; }
        public TimeOnly? OpenAt { get; set; }
        public TimeOnly? CloseAt { get; set; }
    }

    public class FacilityResponse
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FacilityType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FacilityStatus Status { get; set; }
        public TimeOnly? OpenAt { get; set; }
        public TimeOnly? CloseAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FacilityResponse(Facility facility)
        {
            Id = facility.Id;
            HotelId = facility.HotelId;
            Name = facility.Name;
            FacilityType = facility.FacilityType;
            Description = facility.Description;
            Status = facility.Status;
            OpenAt = facility.OpenAt;
            CloseAt = facility.CloseAt;
            CreatedAt = facility.CreatedAt;
            UpdatedAt = facility.UpdatedAt;
        }
    }
}
