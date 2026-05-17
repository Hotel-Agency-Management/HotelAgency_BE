using System.ComponentModel.DataAnnotations;
using Booking.Enums;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateTicketRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public TicketPriority Priority { get; set; }

        [Required]
        public TicketType Type { get; set; }

        [Required]
        public TicketLocationType LocationType { get; set; }

        public int? RoomId { get; set; }

        public int? FacilityId { get; set; }

        public int AssignedToId { get; set; }

        public DateTime Deadline { get; set; }
    }

    public class UpdateTicketRequest
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public TicketPriority? Priority { get; set; }

        public TicketType? Type { get; set; }

        public TicketLocationType? LocationType { get; set; }

        public int? RoomId { get; set; }

        public int? FacilityId { get; set; }

        public int? AssignedToId { get; set; }

        public DateTime? Deadline { get; set; }
    }

    public class UpdateTicketStatusRequest
    {
        [Required]
        public TicketStatus Status { get; set; }
    }

    public class TicketListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        public TicketStatus? Status { get; set; }
        public TicketType? Type { get; set; }
        public TicketPriority? Priority { get; set; }
        public int? AssignedToId { get; set; }
    }

    public class TicketListItemResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public string? LocationLabel { get; set; }
        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TicketListItemResponse(HousekeepingTicket t)
        {
            Id = t.Id;
            Title = t.Title;
            Priority = t.Priority.ToString();
            Status = t.Status.ToString();
            Type = t.Type.ToString();
            LocationType = t.LocationType.ToString();
            LocationLabel = t.LocationType == Enums.TicketLocationType.Room
                ? t.Room?.RoomNumber
                : t.Facility?.Name;
            AssignedToId = t.AssignedToId;
            AssignedToName = t.AssignedTo is null
                ? null
                : $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}";
            Deadline = t.Deadline;
            CreatedAt = t.CreatedAt;
            UpdatedAt = t.UpdatedAt;
        }
    }

    public class TicketDetailResponse
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public int? RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public int CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TicketDetailResponse(HousekeepingTicket t)
        {
            Id = t.Id;
            HotelId = t.HotelId;
            Title = t.Title;
            Description = t.Description;
            Priority = t.Priority.ToString();
            Status = t.Status.ToString();
            Type = t.Type.ToString();
            LocationType = t.LocationType.ToString();
            RoomId = t.RoomId;
            RoomNumber = t.Room?.RoomNumber;
            FacilityId = t.FacilityId;
            FacilityName = t.Facility?.Name;
            AssignedToId = t.AssignedToId;
            AssignedToName = t.AssignedTo is null
                ? null
                : $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}";
            CreatedById = t.CreatedById;
            CreatedByName = t.CreatedBy is null
                ? null
                : $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}";
            Deadline = t.Deadline;
            CreatedAt = t.CreatedAt;
            UpdatedAt = t.UpdatedAt;
        }
    }

    public class TicketBoardResponse
    {
        public List<TicketListItemResponse> Todo { get; set; } = [];
        public List<TicketListItemResponse> InProgress { get; set; } = [];
        public List<TicketListItemResponse> Review { get; set; } = [];
        public List<TicketListItemResponse> Done { get; set; } = [];
    }
}
