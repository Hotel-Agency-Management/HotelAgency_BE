using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Booking.Models;

namespace Booking.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<AgencyDocument> AgencyDocuments { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Agency)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AgencyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Agency>()
                .HasOne(a => a.Owner)
                .WithMany()
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Agency>()
                .HasOne(a => a.Reviewer)
                .WithMany()
                .HasForeignKey(a => a.ReviewedBy)
                .OnDelete(DeleteBehavior.Cascade);

            //Email Verification 
            builder.Entity<EmailVerificationToken>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmailVerificationToken>()
                .HasIndex(x => x.TokenHash)
                .IsUnique();
        }
    }
}
