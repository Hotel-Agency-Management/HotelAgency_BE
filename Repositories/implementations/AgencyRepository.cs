using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;
using Booking.Exceptions;
using Booking.Enums;

namespace Booking.Repositories
{
    public class AgencyRepository(ApplicationDbContext _context) : IAgencyRepository
    {
        public async Task<bool> ExistsByNameAsync(string agencyName)
        {
            return await _context.Agencies
                .AnyAsync(a => a.AgencyName.Trim().ToLower() == agencyName.Trim().ToLower());
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

        /*public async Task<Agency?> GetAgencyByOwnerIdAsync(int userId)
        {
            return await _context.Agencies
                .FirstOrDefaultAsync(a => a.OwnerId == userId);
        }*/
    }
}
