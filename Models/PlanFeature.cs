namespace Booking.Models
{
    public class PlanFeature
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public Plan Plan { get; set; } = null!;
        public ICollection<FeatureLimit> FeatureLimits { get; set; } = new List<FeatureLimit>();
    }
}
