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
            public DbSet<Reservation> Reservations { get; set; }
            public DbSet<ReservationRoom> ReservationRooms { get; set; }
            public DbSet<PaymentLog> PaymentLogs { get; set; }
            public DbSet<TermsAndConditions> TermsAndConditions { get; set; }
            public DbSet<HousekeepingTicket> HousekeepingTickets { get; set; }


            protected override void OnModelCreating(ModelBuilder builder)
            {
                  base.OnModelCreating(builder);

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

                        entity.HasOne(u => u.Hotel)
                        .WithMany(h => h.Users)
                        .HasForeignKey(u => u.HotelId)
                        .IsRequired(false);
                  });

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

                        entity.HasOne(a => a.Owner)
                        .WithMany()
                        .HasForeignKey(a => a.OwnerId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(a => a.Reviewer)
                        .WithMany()
                        .HasForeignKey(a => a.ReviewedBy)
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired(false);
                  });

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

                  builder.Entity<Plan>(e =>
                  {
                        e.HasKey(p => p.Id);
                        e.Property(p => p.Name).IsRequired().HasMaxLength(100);
                        e.Property(p => p.Description).HasColumnType("text");
                        e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                  });

                  builder.Entity<PlanFeature>(e =>
                  {
                        e.HasKey(f => f.Id);
                        e.Property(f => f.FeatureName).IsRequired().HasMaxLength(200);
                        e.HasOne(f => f.Plan)
                        .WithMany(p => p.PlanFeatures)
                        .HasForeignKey(f => f.PlanId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  builder.Entity<FeatureLimit>(e =>
                  {
                        e.HasKey(l => l.Id);
                        e.HasOne(l => l.PlanFeature)
                        .WithMany(f => f.FeatureLimits)
                        .HasForeignKey(l => l.FeatureId);
                  });


                  builder.Entity<Hotel>(entity =>
                  {
                        entity.Property(h => h.CancellationFeePercentage)
                              .HasColumnType("decimal(5,2)")
                              .HasDefaultValue(0m);
                  });

                  builder.Entity<Facility>(entity =>
                  {
                        entity.ToTable("Facilities");

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
                        entity.ToTable("FacilityPhotos");

                        entity.HasOne(p => p.Facility)
                        .WithMany(f => f.Photos)
                        .HasForeignKey(p => p.FacilityId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  builder.Entity<RoomType>(entity =>
                  {
                        entity.HasIndex(r => r.Name).IsUnique();
                  });

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
                        entity.ToTable("RoomPhotos");

                        entity.HasOne(p => p.Room)
                        .WithMany(r => r.Photos)
                        .HasForeignKey(p => p.RoomId)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  builder.Entity<RoomAmenity>(entity =>
                  {
                        entity.HasIndex(a => a.Name)
                        .IsUnique();
                  });

                  builder.Entity<Reservation>(entity =>
                  {
                        entity.ToTable("Reservations");
                        entity.HasIndex(r => r.ReservationNumber).IsUnique();

                        entity.Property(r => r.ReservationNumber).IsRequired().HasMaxLength(30);
                        entity.Property(r => r.Source).HasConversion<string>().HasMaxLength(20);
                        entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
                        entity.Property(r => r.GuestFullName).IsRequired().HasMaxLength(200);
                        entity.Property(r => r.GuestEmail).IsRequired().HasMaxLength(200);
                        entity.Property(r => r.GuestPhone).IsRequired().HasMaxLength(30);
                        entity.Property(r => r.GuestIdNumber).HasMaxLength(100);
                        entity.Property(r => r.ContractPath).HasMaxLength(500);
                        entity.Property(r => r.InvoicePath).HasMaxLength(500);
                        entity.Property(r => r.SpecialRequests).HasMaxLength(1000);
                        entity.Property(r => r.Notes).HasMaxLength(1000);
                        entity.Property(r => r.CancellationFee)
                              .HasColumnType("decimal(18,2)")
                              .HasDefaultValue(0m);
                        entity.Property(r => r.CancellationReason).HasMaxLength(1000);

                        entity.HasOne(r => r.Hotel).WithMany()
                              .HasForeignKey(r => r.HotelId).OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(r => r.Customer).WithMany()
                              .HasForeignKey(r => r.CustomerId)
                              .OnDelete(DeleteBehavior.SetNull).IsRequired(false);

                        entity.HasOne(r => r.CreatedBy).WithMany()
                              .HasForeignKey(r => r.CreatedById).OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(r => r.UpdatedBy).WithMany()
                              .HasForeignKey(r => r.UpdatedById)
                              .OnDelete(DeleteBehavior.SetNull).IsRequired(false);
                  });

                  builder.Entity<PaymentLog>(entity =>
                  {
                        entity.ToTable("PaymentLogs");

                        entity.Property(p => p.Amount)
                              .HasColumnType("decimal(18,2)");

                        entity.Property(p => p.Type)
                              .HasConversion<string>()
                              .HasMaxLength(20);

                        entity.HasOne(p => p.Reservation)
                              .WithMany(r => r.PaymentLogs)
                              .HasForeignKey(p => p.ReservationId)
                              .OnDelete(DeleteBehavior.Cascade);
                  });

                  builder.Entity<ReservationRoom>(entity =>
                  {
                        entity.ToTable("ReservationRooms");
                        entity.HasKey(rr => new { rr.ReservationId, rr.RoomId });

                        entity.HasOne(rr => rr.Reservation).WithMany(r => r.ReservationRooms)
                              .HasForeignKey(rr => rr.ReservationId).OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(rr => rr.Room).WithMany()
                              .HasForeignKey(rr => rr.RoomId).OnDelete(DeleteBehavior.Restrict);
                  });

                  builder.Entity<TermsAndConditions>(entity =>
                  {
                        entity.ToTable("TermsAndConditions");

                        entity.HasOne(t => t.Hotel)
                        .WithMany(h => h.TermsAndConditions)
                        .HasForeignKey(t => t.HotelId)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.Property(t => t.Status)
                        .HasConversion<string>()
                        .HasMaxLength(20);

                        entity.Property(t => t.Content)
                        .HasColumnType("TEXT");

                        entity.Property(t => t.CreatedAt)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                        entity.Property(t => t.UpdatedAt)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .ValueGeneratedOnAddOrUpdate();
                  });

                  builder.Entity<HousekeepingTicket>(entity =>
                  {
                        entity.ToTable("HousekeepingTickets");

                        entity.Property(t => t.Title)
                              .IsRequired()
                              .HasMaxLength(200);

                        entity.Property(t => t.Description)
                              .HasColumnType("TEXT");

                        entity.Property(t => t.Status)
                              .HasConversion<string>()
                              .HasMaxLength(20);

                        entity.Property(t => t.Priority)
                              .HasConversion<string>()
                              .HasMaxLength(20);

                        entity.Property(t => t.Type)
                              .HasConversion<string>()
                              .HasMaxLength(30);

                        entity.Property(t => t.LocationType)
                              .HasConversion<string>()
                              .HasMaxLength(20);

                        entity.HasOne(t => t.Hotel)
                              .WithMany()
                              .HasForeignKey(t => t.HotelId)
                              .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(t => t.AssignedTo)
                              .WithMany()
                              .HasForeignKey(t => t.AssignedToId)
                              .OnDelete(DeleteBehavior.SetNull)
                              .IsRequired(false);

                        entity.HasOne(t => t.CreatedBy)
                              .WithMany()
                              .HasForeignKey(t => t.CreatedById)
                              .OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(t => t.Room)
                              .WithMany()
                              .HasForeignKey(t => t.RoomId)
                              .OnDelete(DeleteBehavior.Restrict)
                              .IsRequired(false);

                        entity.HasOne(t => t.Facility)
                              .WithMany()
                              .HasForeignKey(t => t.FacilityId)
                              .OnDelete(DeleteBehavior.Restrict)
                              .IsRequired(false);
                  });

            }
      }
}
