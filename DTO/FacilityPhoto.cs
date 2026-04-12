using System.ComponentModel.DataAnnotations;
using Booking.Models;

namespace Booking.DTO
{
    public class UploadPhotosRequest
    {
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }

    public class FacilityPhotoResponse
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public FacilityPhotoResponse(FacilityPhoto photo)
        {
            Id = photo.Id;
            FacilityId = photo.FacilityId;
            PhotoUrl = photo.PhotoUrl;
            CreatedAt = photo.CreatedAt;
        }
    }

}
