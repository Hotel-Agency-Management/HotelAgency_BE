using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    [Table("agencies")]
    public class Agency
    {
        public Agency()
        {
            Users = new HashSet<User>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("plan_id")]
        public int PlanId { get; set; }

        [Required]
        [Column("owner_id")]
        public int OwnerId { get; set; }

        [Required]
        [StringLength(150)]
        [Column("agency_name")]
        public string AgencyName { get; set; }

        [Required]
        [StringLength(150)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(50)]
        [Column("phone")]
        public string Phone { get; set; }

        [StringLength(100)]
        [Column("country")]
        public string Country { get; set; }

        [StringLength(100)]
        [Column("city")]
        public string City { get; set; }

        [StringLength(255)]
        [Column("address")]
        public string Address { get; set; }

        [StringLength(255)]
        [Column("logo_url")]
        public string LogoUrl { get; set; }

        [StringLength(20)]
        [Column("primary_color")]
        public string PrimaryColor { get; set; }

        [StringLength(20)]
        [Column("secondary_color")]
        public string SecondaryColor { get; set; }

        [StringLength(20)]
        [Column("tertiary_color")]
        public string TertiaryColor { get; set; }

        [Required]
        [Column("status")]
        public AgencyStatus Status { get; set; }

        [Column("email_verified")]
        public bool EmailVerified { get; set; }

        [Column("reviewed_by")]
        public int? ReviewedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<User> Users { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        [ForeignKey("ReviewedBy")]
        public virtual User Reviewer { get; set; }
    }

    
}