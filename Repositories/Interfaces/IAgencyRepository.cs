using Booking.Models;
namespace Booking.Interfaces.Repositories
{
    public interface IAgencyRepository
    {
        Task<bool> ExistsByNameAsync(string agencyName);
        Task<Agency?> GetByIdAsync(int agencyId);
        Task AddAsync(Agency agency);
        Task UpdateAsync(Agency agency);
        Task DeleteAsync(Agency agency);
    }

}
