using System.Security.Claims;
using Booking.Constants;
using Booking.DTO;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class SystemLogService(
        ISystemLogRepository _logRepository,
        IHotelRepository _hotelRepository,
        IHttpContextAccessor _httpContextAccessor) : ISystemLogService
    {
        public async Task LogAsync(
            string action,
            string entityType,
            int? entityId,
            string description,
            int? agencyId = null,
            int? hotelId = null,
            int? actorId = null,
            string? actorName = null,
            string? actorRole = null)
        {
            try
            {
                if (actorId is null || actorName is null || actorRole is null)
                {
                    var (resolvedId, resolvedName, resolvedRole) = ExtractActorFromClaims();
                    actorId ??= resolvedId;
                    actorName ??= resolvedName;
                    actorRole ??= resolvedRole;
                }

                if (agencyId is null && hotelId.HasValue)
                {
                    var hotel = await _hotelRepository.GetByIdAsync(hotelId.Value);
                    agencyId = hotel?.AgencyId;
                }

                await _logRepository.AddAsync(new SystemLog
                {
                    ActorId = actorId,
                    ActorName = actorName!,
                    ActorRole = actorRole!,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    AgencyId = agencyId,
                    HotelId = hotelId,
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch
            {
                // Logging must never break the main flow
            }
        }

        public async Task<PaginatedResponse<SystemLogResponse>> GetLogsAsync(SystemLogListRequest request)
        {
            var (items, totalCount) = await _logRepository.GetLogsAsync(request);
            return BuildPaginatedResponse(items, request.PageNumber, request.PageSize, totalCount);
        }

        public async Task<PaginatedResponse<SystemLogResponse>> GetAgencyLogsAsync(int agencyId, SystemLogListRequest request)
        {
            var (items, totalCount) = await _logRepository.GetLogsByAgencyAsync(agencyId, request);
            return BuildPaginatedResponse(items, request.PageNumber, request.PageSize, totalCount);
        }

        private static PaginatedResponse<SystemLogResponse> BuildPaginatedResponse(
            IReadOnlyList<SystemLog> items, int pageNumber, int pageSize, int totalCount)
        {
            return new PaginatedResponse<SystemLogResponse>
            {
                Items = items.Select(l => new SystemLogResponse(l)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        private (int? actorId, string actorName, string actorRole) ExtractActorFromClaims()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return (null, "System", "System");

            int? actorId = null;
            if (int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                actorId = id;

            var firstName = user.FindFirstValue(ClaimConstant.FIRST_NAME) ?? string.Empty;
            var lastName = user.FindFirstValue(ClaimConstant.LAST_NAME) ?? string.Empty;
            var actorName = $"{firstName} {lastName}".Trim();
            if (string.IsNullOrEmpty(actorName)) actorName = "Unknown";

            var actorRole = user.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

            return (actorId, actorName, actorRole);
        }
    }
}
