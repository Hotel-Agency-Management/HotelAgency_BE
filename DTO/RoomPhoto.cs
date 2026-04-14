using System.ComponentModel.DataAnnotations;
using Booking.Models;

namespace Booking.DTO
{
    public class UploadRoomPhotosRequest
    {
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }

    public class RoomPhotoResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public RoomPhotoResponse(RoomPhoto photo)
        {
            Id = photo.Id;
            RoomId = photo.RoomId;
            PhotoUrl = photo.PhotoUrl;
            CreatedAt = photo.CreatedAt;
        }
    }
}
