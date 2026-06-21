using Booking.Data;
using Booking.Interfaces.Repositories;
using Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repositories
{
    public class FacilityPhotoRepository(
        ApplicationDbContext _context,
        ILogger<FacilityPhotoRepository> _logger) : IFacilityPhotoRepository
    {
        public async Task<FacilityPhoto> CreateAsync(FacilityPhoto photo)
        {
            _logger.LogDebug("Creating photo for facility {FacilityId}", photo.FacilityId);
            await _context.FacilityPhotos.AddAsync(photo);
            await _context.SaveChangesAsync();
            return photo;
        }

        public async Task<FacilityPhoto?> GetByIdAsync(int photoId)
            => await _context.FacilityPhotos
                .FirstOrDefaultAsync(p => p.Id == photoId);

        public async Task<IEnumerable<FacilityPhoto>> GetAllByFacilityIdAsync(int facilityId)
            => await _context.FacilityPhotos
                .Where(p => p.FacilityId == facilityId)
                .ToListAsync();

        public async Task DeleteAsync(FacilityPhoto photo)
        {
            _context.FacilityPhotos.Remove(photo);
            await _context.SaveChangesAsync();
        }
        
    }
}
