using Booking.DTO;
using Booking.Models;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;

namespace Booking.Interfaces.Services
{
    public class PlanService(IPlanRepository _planRepository) : IPlanService
    {
        public async Task<IEnumerable<PlanDto>> GetPlansAsync(bool includeInactive = false)
        {
            var plans = await _planRepository.GetPlansAsync(includeInactive);
            return plans.Select(p => new PlanDto(p));
        }

        public async Task<PlanDto> GetPlanByIdAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan is null) throw new PlanNotFoundException(id);
            return new PlanDto(plan);
        }

        public async Task<PlanDto> CreatePlanAsync(CreatePlanDto dto)
        {
            if (await _planRepository.NameExistsAsync(dto.Name))
                throw new PlanAlreadyExistsException(dto.Name);

            var plan = new Plan
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PlanFeatures = dto.PlanFeatures.Select(f => new PlanFeature
                {
                    FeatureName = f.FeatureName,
                    IsEnabled = f.IsEnabled,
                    FeatureLimits = f.FeatureLimits.Select(l => new FeatureLimit
                    {
                        LimitValue = l.LimitValue
                    }).ToList()
                }).ToList()
            };

            var created = await _planRepository.CreateAsync(plan);
            return new PlanDto(created);
        }

        public async Task<PlanDto> UpdatePlanAsync(int id, UpdatePlanDto dto)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan is null) throw new PlanNotFoundException(id);

            if (dto.Name is not null)
            {
                if (await _planRepository.NameExistsAsync(dto.Name, excludeId: id))
                    throw new PlanAlreadyExistsException(dto.Name);
                plan.Name = dto.Name;
            }

            if (dto.Description is not null) plan.Description = dto.Description;
            if (dto.Price.HasValue) plan.Price = dto.Price.Value;
            if (dto.IsActive.HasValue) plan.IsActive = dto.IsActive.Value;

            if (dto.PlanFeatures is not null)
            {
                plan.PlanFeatures = dto.PlanFeatures.Select(f => new PlanFeature
                {
                    PlanId = id,
                    FeatureName = f.FeatureName,
                    IsEnabled = f.IsEnabled,
                    FeatureLimits = f.FeatureLimits.Select(l => new FeatureLimit
                    {
                        LimitValue = l.LimitValue
                    }).ToList()
                }).ToList();
            }

            var updated = await _planRepository.UpdateAsync(plan);
            return new PlanDto(updated);
        }

        public async Task DeletePlanAsync(int id)
        {
            if (!await _planRepository.ExistsAsync(id))
                throw new PlanNotFoundException(id);

            await _planRepository.DeleteAsync(id);
        }

        public async Task TogglePlanStatusAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan is null) throw new PlanNotFoundException(id);

            plan.IsActive = !plan.IsActive;
            await _planRepository.UpdateAsync(plan);
        }
    }
}
