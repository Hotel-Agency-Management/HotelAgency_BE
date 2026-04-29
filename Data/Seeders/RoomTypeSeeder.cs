using Booking.Data;
using Booking.Models;

namespace Booking.Data.Seeders
{
    public static class RoomTypeSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.RoomTypes.Any()) return;

            var roomTypes = new List<RoomType>
            {
                new RoomType
                {
                    Id           = 1,
                    Name         = "Standard",
                    Description  = "A comfortable standard room with essential amenities.",
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 2,
                    Name         = "Deluxe",
                    Description  = "A spacious deluxe room with premium furnishings and city views.",
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 3,
                    Name         = "Suite",
                    Description  = "A luxurious suite with a separate living area and premium services.",
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 4,
                    Name         = "Family",
                    Description  = "A large family room designed to accommodate families comfortably.",
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 5,
                    Name         = "Penthouse",
                    Description  = "An exclusive penthouse with panoramic views and top-tier luxury amenities.",
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                }
            };

            await context.RoomTypes.AddRangeAsync(roomTypes);
            await context.SaveChangesAsync();
        }
    }
}
