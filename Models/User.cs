using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Enums;

namespace Booking.Models
{
    [Table("users")]
    public class User
    {
        public User()
        {
            Agencies = new HashSet<Agency>();          // agencies where this user belongs as normal member
            OwnedAgencies = new HashSet<Agency>();     // agencies this user owns
            ReviewedAgencies = new HashSet<Agency>();  // agencies reviewed by this user
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("agency_id")]
        public int? AgencyId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(150)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(50)]
        [Column("phone")]
        public string Phone { get; set; }

        [Required]
        [StringLength(255)]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [Column("role")]
        public UserRole Role { get; set; }

        [Column("email_verified")]
        public bool EmailVerified { get; set; }

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

      
        [ForeignKey("AgencyId")]
        public virtual Agency Agency { get; set; }

        public virtual ICollection<Agency> Agencies { get; set; }

        public virtual ICollection<Agency> OwnedAgencies { get; set; }

        public virtual ICollection<Agency> ReviewedAgencies { get; set; }
    }

}