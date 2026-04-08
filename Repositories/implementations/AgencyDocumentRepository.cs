using Booking.Data;
using Booking.Models;
using Booking.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class AgencyDocumentRepository (ApplicationDbContext _context) : IAgencyDocumentRepository
    {
        public async Task<AgencyDocument?> GetByIdAsync(int id)
        {
            return await _context.AgencyDocuments
                .Include(d => d.Agency)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<AgencyDocument>> GetByAgencyIdAsync(int agencyId)
        {
            return await _context.AgencyDocuments
                .Where(d => d.AgencyId == agencyId)
                .ToListAsync();
        }

        public async Task<AgencyDocument> AddAsync(AgencyDocument document)
        {
            await _context.AgencyDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<AgencyDocument> UpdateAsync(AgencyDocument document)
        {
            _context.AgencyDocuments.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteAsync(AgencyDocument document)
        {
            _context.AgencyDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}
