using System.ComponentModel.DataAnnotations;
using Booking.Models;
namespace Booking.DTO
{
    public class AgencyListRequest
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int Limit { get; set; } = 20;

        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Country { get; set; }
        public bool? EmailVerified { get; set; }
        public string SortOrder { get; set; } = "Oldest";
    }

    public class AgencyStatusCountsResponse
    {
        public int Active { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }
        public int InComplete { get; set; }
        public int Total { get; set; }
    }

    public class CreateAgencyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int OwnerId { get; set; }
    }

    public class CreateAgencyResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UploadAgencyDocumentRequest
    {
        public string DocumentType { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class AgencyDocumentResponse
    {
        public int Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class AgencyProfileResponse
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? TertiaryColor { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int PlanId { get; set; }
        public AgencyProfileResponse(Agency agency)
        {
            Id = agency.Id;
            OwnerId = agency.OwnerId;
            Name = agency.AgencyName;
            Phone = agency.Phone;
            Country = agency.Country;
            City = agency.City;
            LogoUrl = agency.LogoUrl;
            PrimaryColor = agency.PrimaryColor;
            SecondaryColor = agency.SecondaryColor;
            TertiaryColor = agency.TertiaryColor;
            CreatedAt = agency.CreatedAt;
            UpdatedAt = agency.UpdatedAt ?? agency.CreatedAt;
            PlanId = agency.PlanId;
        }
    }

    public class AgencyListItemResponse
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }

        public AgencyListItemResponse(Agency agency)
        {
            OwnerId = agency.OwnerId;
            Name = agency.AgencyName;
            Phone = agency.Phone;
            Country = agency.Country;
            City = agency.City;
            LogoUrl = agency.LogoUrl;
            CreatedAt = agency.CreatedAt;
            Id = agency.Id;
            Email = agency.Owner.Email ?? string.Empty;
            PlanName = agency.Plan.Name;
            Status = agency.Status.ToString();
            EmailVerified = agency.Owner.EmailConfirmed;
        }
    }

    public class UpdateAgencyRequest
    {
        public string? AgencyName { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? TertiaryColor { get; set; }
    }

    public class UploadLogoDto
    {
        public IFormFile File { get; set; } = null!;

    }
    public class AgencyResponseDto
    {
        public string Message { get; set; } = string.Empty;
    }

    public class AgencyInfoDto
    {
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public AgencyInfoDto(Agency agency)
        {
            Name = agency.AgencyName;
            Logo = agency.LogoUrl;
            Phone = agency.Phone;
            Country = agency.Country;
            City = agency.City;
        }
    }

}
