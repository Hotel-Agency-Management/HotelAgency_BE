using Booking.Models;

namespace Booking.Interfaces.Repositories
{
    public record MonthRevenue(int Year, int Month, decimal Revenue);

    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetPlansAsync(bool includeInactive = false);
        Task<Plan?> GetByIdAsync(int id);
        Task<Plan> CreateAsync(Plan plan);
        Task<Plan> UpdateAsync(Plan plan);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task<IEnumerable<MonthRevenue>> GetMonthlySubscriptionRevenueAsync(DateTime from, DateTime to);
    }
}
