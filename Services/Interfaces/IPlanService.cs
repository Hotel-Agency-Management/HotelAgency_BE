using Booking.DTO;
using Booking.Enums;

namespace Booking.Interfaces.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanDto>> GetPlansAsync(bool includeInactive = false);
        Task<PlanDto> GetPlanByIdAsync(int id);
        Task<PlanDto> CreatePlanAsync(CreatePlanDto dto);
        Task<PlanDto> UpdatePlanAsync(int id, UpdatePlanDto dto);
        Task DeletePlanAsync(int id);
    }
}
