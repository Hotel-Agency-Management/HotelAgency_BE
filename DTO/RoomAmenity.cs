using System.ComponentModel.DataAnnotations;
using Booking.Models;

namespace Booking.DTO
{
    public class CreateRoomAmenityRequest
    {
        [Required] public string Name { get; set; } = string.Empty;
    }

    public class RoomAmenityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public RoomAmenityResponse(RoomAmenity amenity)
        {
            Id = amenity.Id;
            Name = amenity.Name;
        }
    }
}
