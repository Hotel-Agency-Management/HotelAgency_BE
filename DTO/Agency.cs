using System.ComponentModel.DataAnnotations;
namespace Booking.DTO
{
     public class AgencyInfoRequest
    {
        [Required] public string AgencyName { get; set; } = string.Empty;
        [Required] public string AgencyAddress { get; set; } = string.Empty;
        [Required] public string TaxNumber { get; set; } = string.Empty;
        [Required] public int SubscriptionPlanId { get; set; }
        public List<IFormFile> Documents { get; set; } = [];
    }
}