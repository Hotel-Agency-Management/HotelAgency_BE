using Booking.Constants;
using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;
using Booking.Exceptions;
using Booking.Enums;
using Booking.DTO;

namespace Booking.Repositories
{
    public class AgencyRepository(
        ApplicationDbContext _context,
        ILogger<AgencyRepository> _logger) : IAgencyRepository
    {
        public async Task<bool> ExistsByNameAsync(string agencyName)
        {
            return await _context.Agencies
                .AnyAsync(a => a.AgencyName.Trim().ToLower() == agencyName.Trim().ToLower());
        }

        public async Task<(IReadOnlyList<Agency> Agencies, int TotalCount)> GetAllAsync(AgencyListRequest request)
        {
            _logger.LogDebug("Querying agencies: search={Search}, status={Status}, page={Page}", request.Search, request.Status, request.Page);
            var query = _context.Agencies
                .AsNoTracking()
                .Include(a => a.Owner)
                .Include(a => a.Plan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.AgencyName.ToLower().Contains(search) ||
                    (a.Owner.Email != null && a.Owner.Email.ToLower().Contains(search)) ||
                    a.City.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<AgencyStatus>(request.Status.Trim(), true, out var status))
                    query = query.Where(a => a.Status == status);
                else
                    query = query.Where(a => false);
            }

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                var country = request.Country.Trim().ToLower();
                query = query.Where(a => a.Country.ToLower() == country);
            }

            if (request.EmailVerified.HasValue)
                query = query.Where(a => a.Owner.EmailConfirmed == request.EmailVerified.Value);

            var totalCount = await query.CountAsync();

            var isDescending = request.SortOrder.Equals(SortOrders.Newest, StringComparison.OrdinalIgnoreCase);
            var agencies = await (isDescending
                    ? query.OrderByDescending(a => a.Id)
                    : query.OrderBy(a => a.Id))
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            return (agencies, totalCount);
        }

        public async Task<AgencyStatusCountsResponse> GetStatusCountsAsync()
        {
            var counts = await _context.Agencies
                .AsNoTracking()
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return new AgencyStatusCountsResponse
            {
                Active = counts.FirstOrDefault(c => c.Status == AgencyStatus.Active)?.Count ?? 0,
                Rejected = counts.FirstOrDefault(c => c.Status == AgencyStatus.Rejected)?.Count ?? 0,
                Pending = counts.FirstOrDefault(c => c.Status == AgencyStatus.Pending)?.Count ?? 0,
                InComplete = counts.FirstOrDefault(c => c.Status == AgencyStatus.InComplete)?.Count ?? 0,
                Total = counts.Sum(c => c.Count)
            };
        }

        public async Task<Agency?> GetByIdAsync(int agencyId)
        {
            return await _context.Agencies
                .FirstOrDefaultAsync(a => a.Id == agencyId);
        }

        public async Task AddAsync(Agency agency)
        {
            await _context.Agencies.AddAsync(agency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Agency agency)
        {
            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Agency agency)
        {
            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateStatusAsync(int agencyId, AgencyStatus status)
        {
            var agency = await _context.Agencies.FindAsync(agencyId);
            if (agency == null)
                throw new AgencyNotFoundException(agencyId);

            agency.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MonthlyAgencyCount>> GetMonthlyGrowthAsync(DateTime from, DateTime to)
            => await _context.Agencies
                .Where(a => a.CreatedAt >= from && a.CreatedAt <= to)
                .GroupBy(a => new { a.CreatedAt.Year, a.CreatedAt.Month })
                .Select(g => new MonthlyAgencyCount(g.Key.Year, g.Key.Month, g.Count()))
                .ToListAsync();
    }
}
