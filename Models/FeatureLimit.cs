namespace Booking.Models
{
    public class FeatureLimit
    {
        public int Id { get; set; }
        public int FeatureId { get; set; }
        public int LimitValue { get; set; }

        public PlanFeature PlanFeature { get; set; } = null!;
    }
}