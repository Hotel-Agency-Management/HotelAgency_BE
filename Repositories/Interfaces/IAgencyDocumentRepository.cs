using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IAgencyDocumentRepository
    {
        Task<AgencyDocument?> GetByIdAsync(int id);
        Task<IEnumerable<AgencyDocument>> GetByAgencyIdAsync(int agencyId);
        Task<AgencyDocument> AddAsync(AgencyDocument document);
        Task<AgencyDocument> UpdateAsync(AgencyDocument document);
        Task DeleteAsync(AgencyDocument document);
        Task<int> CountByAgencyIdAsync(int agencyId);
    }
}
