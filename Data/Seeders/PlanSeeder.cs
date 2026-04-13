using Booking.Data;
using Booking.Models;
using Booking.Enums;

namespace Booking.Data.Seeders
{
    public static class PlanSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Plans.Any()) return;

            var plans = new List<Plan>
            {
                new Plan
                {
                    Id          = 1,
                    Name         = "Free",
                    Description  = "Default plan for any new agency joining the platform.",
                    Price        = 0,
                    Status     = PlanStatus.Active,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                    PlanFeatures = new List<PlanFeature>
                    {
                        new PlanFeature
                        {
                            FeatureName = "Team Members",
                            IsEnabled   = true,
                            FeatureLimits = new List<FeatureLimit>
                            {
                                new FeatureLimit { LimitValue = 12 }
                            }
                        },
                        new PlanFeature
                        {
                            FeatureName = "Hotels",
                            IsEnabled   = true,
                            FeatureLimits = new List<FeatureLimit>
                            {
                                new FeatureLimit {LimitValue = 2 }
                            }
                        },
                        new PlanFeature { FeatureName = "API Access",       IsEnabled = false },
                        new PlanFeature { FeatureName = "Reports",          IsEnabled = false },
                        new PlanFeature { FeatureName = "Priority Support", IsEnabled = false }
                    }
                },
                new Plan
                {
                    Id          = 2,
                    Name         = "Basic",
                    Description  = "Essential features for small agencies.",
                    Price        = 29.99m,
                    Status     = PlanStatus.Active,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                    PlanFeatures = new List<PlanFeature>
                    {
                        new PlanFeature
                        {
                            FeatureName = "Team Members",
                            IsEnabled   = true,
                            FeatureLimits = new List<FeatureLimit>
                            {
                                new FeatureLimit { LimitValue = 20 }
                            }
                        },
                        new PlanFeature
                        {
                            FeatureName = "Hotels",
                            IsEnabled   = true,
                            FeatureLimits = new List<FeatureLimit>
                            {
                                new FeatureLimit { LimitValue = 5 }
                            }
                        },
                        new PlanFeature { FeatureName = "Reports", IsEnabled = false },
                        new PlanFeature { FeatureName = "Priority Support", IsEnabled = false }
                    }
                }
            };

            await context.Plans.AddRangeAsync(plans);
            await context.SaveChangesAsync();
        }
    }
}
