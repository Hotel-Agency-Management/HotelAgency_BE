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
            public DbSet<Plan> Plans { get; set; }
            public DbSet<PlanFeature> PlanFeatures { get; set; }
            public DbSet<FeatureLimit> FeatureLimits { get; set; }
            public DbSet<Hotel> Hotels { get; set; }
            public DbSet<Facility> Facilities { get; set; }
            public DbSet<FacilityPhoto> FacilityPhotos { get; set; }
            public DbSet<RoomType> RoomTypes { get; set; }
            public DbSet<Room> Rooms { get; set; }
            public DbSet<RoomPhoto> RoomPhotos { get; set; }
            public DbSet<RoomAmenity> RoomAmenities { get; set; }


            protected override void OnModelCreating(ModelBuilder builder)
            {
                  base.OnModelCreating(builder);

                  // ApplicationUser
                  builder.Entity<ApplicationUser>(entity =>
                  {
                        entity.ToTable("Users");

                        entity.Property(u => u.FirstName)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(u => u.LastName)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(u => u.Gender)
                        .HasMaxLength(20);

                        entity.HasOne(u => u.Agency)
                        .WithMany(a => a.Users)
                        .HasForeignKey(u => u.AgencyId)
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired(false);
                  });

                  // Agency
                  builder.Entity<Agency>(entity =>
                  {
                        entity.ToTable("Agencies");

                        entity.Property(a => a.AgencyName)
                        .IsRequired()
                        .HasMaxLength(200);

                        entity.Property(a => a.Phone)
                        .IsRequired()
                        .HasMaxLength(30);

                        entity.Property(a => a.Country)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(a => a.City)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(a => a.LogoUrl)
                        .HasMaxLength(500);

                        entity.Property(a => a.PrimaryColor)
                        .HasMaxLength(20);

                        entity.Property(a => a.SecondaryColor)
                        .HasMaxLength(20);

                        entity.Property(a => a.TertiaryColor)
                        .HasMaxLength(20);

                        entity.Property(a => a.Status)
                        .HasConversion<string>()
                        .HasMaxLength(30);

                        // Agency
                        entity.HasOne(a => a.Owner)
                        .WithMany()
                        .HasForeignKey(a => a.OwnerId)
                        .OnDelete(DeleteBehavior.Cascade);

                        // Agency → Reviewer (optional admin user)
                        entity.HasOne(a => a.Reviewer)
                        .WithMany()
                        .HasForeignKey(a => a.ReviewedBy)
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired(false);
                  });

                  // AgencyDocument
                  builder.Entity<AgencyDocument>(entity =>
                  {
                        entity.ToTable("AgencyDocuments");

                        entity.Property(d => d.DocumentType)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(d => d.FilePath)
                        .IsRequired()
                        .HasMaxLength(500);

                        entity.HasOne(d => d.Agency)
                        .WithMany(a => a.Documents)
                        .HasForeignKey(d => d.AgencyId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // RefreshToken
                  builder.Entity<RefreshToken>(entity =>
                  {
                        entity.ToTable("RefreshTokens");

                        entity.Property(t => t.Token)
                        .IsRequired()
                        .HasMaxLength(500);

                        entity.HasIndex(t => t.Token)
                        .IsUnique();

                        entity.HasOne(t => t.User)
                        .WithMany()
                        .HasForeignKey(t => t.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // PasswordResetCode 
                  builder.Entity<PasswordResetCode>(entity =>
                  {
                        entity.ToTable("PasswordResetCodes");

                        entity.Property(p => p.Code)
                        .IsRequired()
                        .HasMaxLength(10);

                        entity.HasOne(p => p.User)
                        .WithMany()
                        .HasForeignKey(p => p.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // EmailVerificationToken
                  builder.Entity<EmailVerificationToken>(entity =>
                  {
                        entity.ToTable("EmailVerificationTokens");

                        entity.Property(t => t.TokenHash)
                        .IsRequired()
                        .HasMaxLength(500);

                        entity.HasIndex(t => t.TokenHash)
                        .IsUnique();

                        entity.HasOne(t => t.User)
                        .WithMany()
                        .HasForeignKey(t => t.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  //plan
                  builder.Entity<Plan>(e =>
                  {
                        e.HasKey(p => p.Id);
                        e.Property(p => p.Name).IsRequired().HasMaxLength(100);
                        e.Property(p => p.Description).HasColumnType("text");
                        e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                  });

                  //PlanFeature
                  builder.Entity<PlanFeature>(e =>
                  {
                        e.HasKey(f => f.Id);
                        e.Property(f => f.FeatureName).IsRequired().HasMaxLength(200);
                        e.HasOne(f => f.Plan)
                        .WithMany(p => p.PlanFeatures)
                        .HasForeignKey(f => f.PlanId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  //FeatureLimit 
                  builder.Entity<FeatureLimit>(e =>
                  {
                        e.HasKey(l => l.Id);
                        e.HasOne(l => l.PlanFeature)
                        .WithMany(f => f.FeatureLimits)
                        .HasForeignKey(l => l.FeatureId);
                  });
                  //Facility
                  builder.Entity<Facility>(entity =>
                  {
                        entity.HasOne(f => f.Hotel)
                        .WithMany(h => h.Facilities)
                        .HasForeignKey(f => f.HotelId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.Property(f => f.Status)
                        .HasConversion<string>()
                        .HasMaxLength(30);
                  });

                  builder.Entity<FacilityPhoto>(entity =>
                  {
                        entity.HasOne(p => p.Facility)
                        .WithMany(f => f.Photos)
                        .HasForeignKey(p => p.FacilityId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  //Room Type 
                  builder.Entity<RoomType>(entity =>
                  {
                        entity.HasIndex(r => r.Name).IsUnique();
                  });

                  // Room
                  builder.Entity<Room>(entity =>
                  {
                        entity.HasOne(r => r.Hotel)
                        .WithMany(h => h.Rooms)
                        .HasForeignKey(r => r.HotelId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(r => r.RoomType)
                        .WithMany(rt => rt.Rooms)
                        .HasForeignKey(r => r.RoomTypeId)
                        .OnDelete(DeleteBehavior.Restrict);

                        entity.Property(r => r.Status)
                        .HasConversion<string>()
                        .HasMaxLength(20);

                        entity.HasIndex(r => new { r.RoomNumber, r.HotelId })
                        .IsUnique();

                        entity.HasMany(r => r.Amenities)
                        .WithMany(a => a.Rooms)
                        .UsingEntity(j => j.ToTable("RoomAmenityMaps"));
                  });

                  builder.Entity<RoomPhoto>(entity =>
                  {
                        entity.HasOne(p => p.Room)
                        .WithMany(r => r.Photos)
                        .HasForeignKey(p => p.RoomId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Room Amenity
                  builder.Entity<RoomAmenity>(entity =>
                  {
                        entity.HasIndex(a => a.Name)
                        .IsUnique();
                  });
            }
      }
}
