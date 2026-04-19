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
                    Capacity     = 2,
                    DailyPrice   = 50.00m,
                    WeeklyPrice  = 300.00m,
                    MonthlyPrice = 1000.00m,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 2,
                    Name         = "Deluxe",
                    Description  = "A spacious deluxe room with premium furnishings and city views.",
                    Capacity     = 2,
                    DailyPrice   = 100.00m,
                    WeeklyPrice  = 600.00m,
                    MonthlyPrice = 2000.00m,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 3,
                    Name         = "Suite",
                    Description  = "A luxurious suite with a separate living area and premium services.",
                    Capacity     = 4,
                    DailyPrice   = 200.00m,
                    WeeklyPrice  = 1200.00m,
                    MonthlyPrice = 4000.00m,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 4,
                    Name         = "Family",
                    Description  = "A large family room designed to accommodate families comfortably.",
                    Capacity     = 6,
                    DailyPrice   = 150.00m,
                    WeeklyPrice  = 900.00m,
                    MonthlyPrice = 3000.00m,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                },
                new RoomType
                {
                    Id           = 5,
                    Name         = "Penthouse",
                    Description  = "An exclusive penthouse with panoramic views and top-tier luxury amenities.",
                    Capacity     = 4,
                    DailyPrice   = 500.00m,
                    WeeklyPrice  = 3000.00m,
                    MonthlyPrice = 10000.00m,
                    CreatedAt    = DateTime.UtcNow,
                    UpdatedAt    = DateTime.UtcNow,
                }
            };

            await context.RoomTypes.AddRangeAsync(roomTypes);
            await context.SaveChangesAsync();
        }
    }
}
