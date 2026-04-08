using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;


namespace Booking.Repositories
{
    public class PlanRepository(ApplicationDbContext _context) : IPlanRepository
    {
        public async Task<IEnumerable<Plan>> GetPlansAsync(bool includeInactive = false)
        {
            var query = _context.Plans
                .Include(p => p.PlanFeatures)
                    .ThenInclude(f => f.FeatureLimits)
                .AsQueryable();

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            return await query.OrderBy(p => p.Price).ToListAsync();
        }

        public async Task<Plan?> GetByIdAsync(int id)
        {
            return await _context.Plans
                .Include(p => p.PlanFeatures)
                    .ThenInclude(f => f.FeatureLimits)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Plan> CreateAsync(Plan plan)
        {
            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> UpdateAsync(Plan plan)
        {
            plan.UpdatedAt = DateTime.UtcNow;
            _context.Plans.Update(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan is null) return false;

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Plans.AnyAsync(p => p.Id == id);

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
            => await _context.Plans.AnyAsync(p =>
                p.Name.ToLower() == name.ToLower() &&
                (excludeId == null || p.Id != excludeId));
    }
}
