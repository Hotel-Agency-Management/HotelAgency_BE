using System.ComponentModel.DataAnnotations;
using Booking.Models;

namespace Booking.DTO
{
    public class HotelListRequest
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
        public string? Location { get; set; }
    }


    public class CreateHotelRequest
    {
        [Required] public int AgencyId { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Country { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        [Required] public string Currency { get; set; } = string.Empty;
        [Required] public IFormFile Logo { get; set; } = null!;
        [Required] public string PrimaryColor { get; set; } = string.Empty;
        [Required] public string SecondaryColor { get; set; } = string.Empty;
        [Required] public string TertiaryColor { get; set; } = string.Empty;
        [Required] public int ManagerUserId { get; set; }
        [Required] public string Phone { get; set; } = string.Empty;
        [Required] public IFormFile CoverPhoto { get; set; } = null!;
    }

    public class UpdateHotelRequest
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Currency { get; set; }
        public IFormFile? Logo { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? TertiaryColor { get; set; }
        public int? ManagerUserId { get; set; }
        public string? Phone { get; set; }
        public IFormFile? CoverPhoto { get; set; }
    }

    public class HotelResponse
    {
        public int Id { get; set; }
        public int AgencyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string CoverPath { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string TertiaryColor { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int ManagerUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public HotelResponse(Hotel hotel)
        {
            Id = hotel.Id;
            AgencyId = hotel.AgencyId;
            Name = hotel.Name;
            Country = hotel.Country;
            City = hotel.City;
            Address = hotel.Address;
            Currency = hotel.Currency;
            LogoUrl = hotel.LogoUrl;
            CoverPath = hotel.CoverPath;
            PrimaryColor = hotel.PrimaryColor;
            SecondaryColor = hotel.SecondaryColor;
            TertiaryColor = hotel.TertiaryColor;
            Phone = hotel.Phone;
            ManagerUserId = hotel.ManagerUserId;
            CreatedAt = hotel.CreatedAt;
            UpdatedAt = hotel.UpdatedAt;
        }
    }

    public class HotelInfoDto
    {
        public string? HotelName { get; set; }
        public string? HotelLogo { get; set; }
        public string? HotelCountry { get; set; }
        public string? HotelCity { get; set; }
        public string? Phone { get; set; }

        public HotelInfoDto(Hotel hotel)
        {
            HotelName = hotel.Name;
            HotelLogo = hotel.LogoUrl;
            HotelCountry = hotel.Country;
            HotelCity = hotel.City;
            Phone = hotel.Phone;
        }
    }
}
