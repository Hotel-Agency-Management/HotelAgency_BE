using Booking.DTO;
using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface ISystemLogRepository
    {
        Task AddAsync(SystemLog log);
        Task<(IReadOnlyList<SystemLog> Items, int TotalCount)> GetLogsAsync(SystemLogListRequest request);
        Task<(IReadOnlyList<SystemLog> Items, int TotalCount)> GetLogsByAgencyAsync(int agencyId, SystemLogListRequest request);
    }
}
