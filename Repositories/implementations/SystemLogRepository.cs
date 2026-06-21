using Booking.Data;
using Booking.DTO;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class SystemLogRepository(ApplicationDbContext _context) : ISystemLogRepository
    {
        public async Task AddAsync(SystemLog log)
        {
            await _context.SystemLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<SystemLog> Items, int TotalCount)> GetLogsAsync(SystemLogListRequest request)
        {
            var query = BuildQuery(request);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private IQueryable<SystemLog> BuildQuery(SystemLogListRequest request)
        {
            var query = _context.SystemLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Action))
                query = query.Where(l => l.Action == request.Action);

            if (!string.IsNullOrWhiteSpace(request.EntityType))
                query = query.Where(l => l.EntityType == request.EntityType);

            if (request.ActorId.HasValue)
                query = query.Where(l => l.ActorId == request.ActorId.Value);

            if (request.AgencyId.HasValue)
                query = query.Where(l => l.AgencyId == request.AgencyId.Value);

            if (request.HotelId.HasValue)
                query = query.Where(l => l.HotelId == request.HotelId.Value);

            if (request.From.HasValue)
                query = query.Where(l => l.CreatedAt >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(l => l.CreatedAt <= request.To.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(l => l.Description.Contains(request.Search));

            return query;
        }
    }
}
