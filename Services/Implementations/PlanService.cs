using Booking.DTO;
using Booking.Models;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Enums;
using System.Globalization;

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
                Status = dto.Status,
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
            if (dto.Status.HasValue) plan.Status = dto.Status.Value;

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

        public async Task<RevenueOverviewResponse> GetRevenueOverviewAsync()
        {
            var now = DateTime.UtcNow;
            var windowStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11);
            var windowEnd   = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1).AddTicks(-1);

            var raw = (await _planRepository.GetMonthlySubscriptionRevenueAsync(windowStart, windowEnd))
                .ToDictionary(r => (r.Year, r.Month), r => r.Revenue);

            var months = Enumerable.Range(0, 12)
                .Select(i => windowStart.AddMonths(i))
                .Select(d => new MonthlyRevenueItemForDashBoard
                {
                    Month  = d.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    Amount = raw.TryGetValue((d.Year, d.Month), out var amt) ? amt : 0m
                })
                .ToList();

            return new RevenueOverviewResponse { Revenue = months };
        }

        public async Task<decimal> GetTotalSubscriptionRevenueAsync()
            => await _planRepository.GetTotalSubscriptionRevenueAsync();

        public async Task<SubscriptionDistributionResponse> GetSubscriptionDistributionAsync()
        {
            var items = (await _planRepository.GetSubscriptionDistributionAsync()).ToList();
            return new SubscriptionDistributionResponse
            {
                Total     = items.Sum(i => i.Count),
                Breakdown = items.Select(i => new SubscriptionSlice
                {
                    PlanName = i.PlanName,
                    Count    = i.Count
                }).ToList()
            };
        }
    }
}
