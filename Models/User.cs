using Microsoft.AspNetCore.Identity;
using System;

namespace Booking.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int? AgencyId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; } 

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Agency? Agency { get; set; }
    }
}
