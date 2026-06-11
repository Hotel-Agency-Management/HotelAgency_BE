namespace Booking.DTO
{
    public class SystemLogResponse
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
        public DateTime CreatedAt { get; set; }

        public SystemLogResponse(Models.SystemLog log)
        {
            Id = log.Id;
            ActorId = log.ActorId;
            ActorName = log.ActorName;
            ActorRole = log.ActorRole;
            Action = log.Action;
            EntityType = log.EntityType;
            EntityId = log.EntityId;
            Description = log.Description;
            AgencyId = log.AgencyId;
            HotelId = log.HotelId;
            CreatedAt = log.CreatedAt;
        }
    }

    public class SystemLogListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public int? ActorId { get; set; }
        public int? AgencyId { get; set; }
        public int? HotelId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Search { get; set; }
    }
}
