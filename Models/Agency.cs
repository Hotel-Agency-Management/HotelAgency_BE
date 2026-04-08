using System;
using System.Collections.Generic;
using Booking.Enums;

namespace Booking.Models
{
    public class Agency
    {
        public int Id { get; set; }
        //public int PlanId { get; set; }
        //public Plan Plan { get; set; } = null!;
        public int OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;
        public AgencyStatus Status { get; set; } = AgencyStatus.Pending;

        public string AgencyName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? TertiaryColor { get; set; }

        public int? ReviewedBy { get; set; }
        public ApplicationUser? Reviewer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<AgencyDocument> Documents { get; set; } = new List<AgencyDocument>();
    }
}
