using Booking.Data;
using Booking.Models;

namespace Booking.Data.Seeders
{
    public static class RoomAmenitySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.RoomAmenities.Any()) return;

            var roomAmenities = new List<RoomAmenity>
            {
                new RoomAmenity
                {
                    Id   = 1,
                    Name = "Free Wi-Fi"
                },
                new RoomAmenity
                {
                    Id   = 2,
                    Name = "Air Conditioning"
                },
                new RoomAmenity
                {
                    Id   = 3,
                    Name = "Television"
                },
                new RoomAmenity
                {
                    Id   = 4,
                    Name = "Mini Bar"
                },
                new RoomAmenity
                {
                    Id   = 5,
                    Name = "Private Bathroom"
                },
                new RoomAmenity
                {
                    Id   = 6,
                    Name = "Balcony"
                },
                new RoomAmenity
                {
                    Id   = 7,
                    Name = "Room Service"
                },
                new RoomAmenity
                {
                    Id   = 8,
                    Name = "Coffee Maker"
                },
                new RoomAmenity
                {
                    Id   = 9,
                    Name = "Safe Box"
                },
                new RoomAmenity
                {
                    Id   = 10,
                    Name = "Swimming Pool Access"
                }
            };

            await context.RoomAmenities.AddRangeAsync(roomAmenities);
            await context.SaveChangesAsync();
        }
    }
}
