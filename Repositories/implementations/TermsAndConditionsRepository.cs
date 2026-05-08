using Booking.Data;
using Booking.Enums;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class TermsAndConditionsRepository(ApplicationDbContext _context) : ITermsAndConditionsRepository
    {
        public async Task<TermsAndConditions> CreateAsync(TermsAndConditions terms)
        {
            await _context.TermsAndConditions.AddAsync(terms);
            await _context.SaveChangesAsync();
            return terms;
        }

        public async Task<TermsAndConditions?> GetByIdAsync(int id)
            => await _context.TermsAndConditions.FindAsync(id);

        public async Task<IEnumerable<TermsAndConditions>> GetAllByHotelIdAsync(int hotelId)
            => await _context.TermsAndConditions
                .Where(t => t.HotelId == hotelId)
                .ToListAsync();

        public async Task<TermsAndConditions> UpdateAsync(TermsAndConditions terms)
        {
            _context.TermsAndConditions.Update(terms);
            await _context.SaveChangesAsync();
            return terms;
        }

        public async Task SetAllToInactiveForHotelAsync(int hotelId, int? excludeId = null)
        {
            await _context.TermsAndConditions
                .Where(t => t.HotelId == hotelId
                    && t.Status == TermsStatus.Active
                    && (excludeId == null || t.Id != excludeId))
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.Status, TermsStatus.Draft));
        }
    }
}
