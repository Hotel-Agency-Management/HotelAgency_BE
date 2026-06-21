namespace Booking.Models
{
    public class SystemLog
    {
        public int Id { get; set; }
        public int? ActorId { get; set; }
        public string ActorName { get; set; } = string.Empty;
        public string ActorRole { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? AgencyId { get; set; }
        public int? HotelId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser? Actor { get; set; }
    }
}
