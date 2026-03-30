using System;
using System.Collections.Generic;

namespace Booking.Models
{
    public class Agency
    {
        public int Id { get; set; }

        //public int PlanId { get; set; }
        //public Plan Plan { get; set; } = null!;

        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;

        public string AgencyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string LogoUrl { get; set; } = string.Empty;

        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string TertiaryColor { get; set; } = string.Empty;

        public bool EmailVerified { get; set; }

        public string? ReviewedBy { get; set; }
        public ApplicationUser? Reviewer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }

}
