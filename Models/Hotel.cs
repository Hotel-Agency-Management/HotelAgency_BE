using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Constants;

namespace Booking.Models
{

        public class Hotel
        {
                [Key]
                public int Id { get; set; }

                [Required]
                public int AgencyId { get; set; }

                [Required]
                public string Name { get; set; } = string.Empty;

                [Required]
                public string Phone { get; set; } = string.Empty;

                [Required]
                public string CoverPath { get; set; } = string.Empty;

                [Required]
                public string Country { get; set; } = string.Empty;

                [Required]
                public string City { get; set; } = string.Empty;

                [Required]
                public string Address { get; set; } = string.Empty;

                [Required]
                public string Currency { get; set; } = string.Empty;

                [Required]
                public string LogoUrl { get; set; } = string.Empty;

                [Required]
                public string PrimaryColor { get; set; } = string.Empty;

                [Required]
                public string SecondaryColor { get; set; } = string.Empty;

                [Required]
                public string TertiaryColor { get; set; } = string.Empty;

                [Required]
                public int ManagerUserId { get; set; }

                public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

                public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
                
                public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
        }
}
