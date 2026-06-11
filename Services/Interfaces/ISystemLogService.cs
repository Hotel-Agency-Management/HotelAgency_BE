using Booking.DTO;

namespace Booking.Interfaces.Services
{
    public interface ISystemLogService
    {
        Task LogAsync(
            string action,
            string entityType,
            int? entityId,
            string description,
            int? agencyId = null,
            int? hotelId = null,
            int? actorId = null,
            string? actorName = null,
            string? actorRole = null);

        Task<PaginatedResponse<SystemLogResponse>> GetLogsAsync(SystemLogListRequest request);
    }
}
