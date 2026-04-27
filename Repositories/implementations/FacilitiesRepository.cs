using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class FacilityRepository(ApplicationDbContext _context) : IFacilityRepository
    {
        public async Task<Facility> CreateAsync(Facility facility)
        {
            await _context.Facilities.AddAsync(facility);
            await _context.SaveChangesAsync();
            return facility;
        }

        public async Task<Facility?> GetByIdAsync(int facilityId)
            => await _context.Facilities.FindAsync(facilityId);

        public async Task<IEnumerable<Facility>> GetAllByHotelIdAsync(int hotelId)
            => await _context.Facilities
                .Where(f => f.HotelId == hotelId)
                .ToListAsync();

        public async Task<bool> ExistsByNameAndHotelIdAsync(string name, int hotelId)
            => await _context.Facilities
                .AnyAsync(f => f.Name == name && f.HotelId == hotelId);

        public async Task<Facility> UpdateAsync(Facility facility)
        {
            _context.Facilities.Update(facility);
            await _context.SaveChangesAsync();
            return facility;
        }

        public async Task DeleteAsync(Facility facility)
        {
            _context.Facilities.Remove(facility);
            await _context.SaveChangesAsync();
        }
    }
}
