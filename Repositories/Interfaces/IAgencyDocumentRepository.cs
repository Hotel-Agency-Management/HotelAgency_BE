using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public interface IAgencyDocumentRepository
    {
        Task AddAsync(AgencyDocument document);
        Task<List<AgencyDocument>> GetByAgencyIdAsync(int agencyId);
        Task DeleteRangeAsync(List<AgencyDocument> documents);
    }
}
