using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class AgencyDocumentRepository(ApplicationDbContext _context) : IAgencyDocumentRepository
    {
        public async Task AddAsync(AgencyDocument document)
        {
            await _context.AgencyDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AgencyDocument>> GetByAgencyIdAsync(int agencyId)
        {
            return await _context.AgencyDocuments
                .Where(d => d.AgencyId == agencyId)
                .ToListAsync();
        }
    }
}
