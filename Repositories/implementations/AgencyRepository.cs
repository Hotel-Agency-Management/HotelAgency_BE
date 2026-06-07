using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;
using Booking.Exceptions;
using Booking.Enums;
using Booking.DTO;

namespace Booking.Repositories
{
    public class AgencyRepository(ApplicationDbContext _context) : IAgencyRepository
    {
        public async Task<bool> ExistsByNameAsync(string agencyName)
        {
            return await _context.Agencies
                .AnyAsync(a => a.AgencyName.Trim().ToLower() == agencyName.Trim().ToLower());
        }

        public async Task<(IReadOnlyList<Agency> Agencies, int TotalCount)> GetAllAsync(AgencyListRequest request)
        {
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

            var agencies = await query
                .OrderBy(a => a.Id)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            return (agencies, totalCount);
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
    }
}
