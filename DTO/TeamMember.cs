using System.ComponentModel.DataAnnotations;
using Booking.Models;
using Booking.Enums;

namespace Booking.DTO
{
    public class TeamMemberListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? Role { get; set; }

        public bool? Assigned { get; set; }
    }

    public class CreateTeamMemberRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class AssignTeamMemberRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateTeamMemberRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public int? HotelId { get; set; }
    }

    public class TeamMemberResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool CanBeHotelManager { get; set; }
        public int hotelId { get; set; }

        public TeamMemberResponse(ApplicationUser user, string role)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email ?? string.Empty;
            PhoneNumber = user.PhoneNumber;
            Role = role;
            CanBeHotelManager = role == Constants.Roles.PropertyManager;
            hotelId = user.HotelId!.Value;
        }
    }

    public class PaginatedResponse<T>
    {
        public IReadOnlyCollection<T> Items { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
