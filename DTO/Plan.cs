using Booking.Models;
using Booking.Enums;

namespace Booking.DTO
{

    public class CreateFeatureLimitDto
    {
        public int LimitValue { get; set; }
    }

    public class CreatePlanFeatureDto
    {
        public string FeatureName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public List<CreateFeatureLimitDto> FeatureLimits { get; set; } = new();
    }

    public class PlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public PlanStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PlanFeatureDto> PlanFeatures { get; set; } = new();

        public PlanDto(Plan plan)
        {
            Id = plan.Id;
            Name = plan.Name;
            Description = plan.Description;
            Price = plan.Price;
            Status = plan.Status;
            CreatedAt = plan.CreatedAt;
            UpdatedAt = plan.UpdatedAt;
            PlanFeatures = plan.PlanFeatures
                .Select(f => new PlanFeatureDto(f))
                .ToList();
        }
    }

    public class PlanFeatureDto
    {
        public int Id { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public List<FeatureLimitDto> FeatureLimits { get; set; } = new();

        public PlanFeatureDto(PlanFeature feature)
        {
            Id = feature.Id;
            FeatureName = feature.FeatureName;
            IsEnabled = feature.IsEnabled;
            FeatureLimits = feature.FeatureLimits
                .Select(l => new FeatureLimitDto(l))
                .ToList();
        }
    }

    public class FeatureLimitDto
    {
        public int Id { get; set; }
        public int LimitValue { get; set; }
        public FeatureLimitDto(FeatureLimit limit)
        {
            Id = limit.Id;
            LimitValue = limit.LimitValue;
        }
    }

    public class CreatePlanDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public PlanStatus Status { get; set; }
        public List<CreatePlanFeatureDto> PlanFeatures { get; set; } = new();
    }

    public class UpdatePlanDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public PlanStatus? Status { get; set; }
        public List<CreatePlanFeatureDto>? PlanFeatures { get; set; }
    }
}
