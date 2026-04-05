using System.ComponentModel.DataAnnotations;
using Booking.Models;
namespace Booking.DTO
{
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

    public class UploadDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
    }

    public class AgencyDocumentResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
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

}
