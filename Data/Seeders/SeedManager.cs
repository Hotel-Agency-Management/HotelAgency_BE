using Microsoft.AspNetCore.Identity;
namespace Booking.Data.Seeders
{
    public static class SeedManager
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            await RoleSeeder.SeedAsync(roleManager);

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await PlanSeeder.SeedAsync(context);

            var RoomsType = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await RoomTypeSeeder.SeedAsync(RoomsType);

            var RoomsAmenity = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await RoomAmenitySeeder.SeedAsync(RoomsAmenity);
        }
    }
}
