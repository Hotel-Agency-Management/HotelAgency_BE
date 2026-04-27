using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Booking.Data.Seeders
{
    public static class SeedManager
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            await RoleSeeder.SeedAsync(roleManager);
            await PlanSeeder.SeedAsync(context);
            await RoomTypeSeeder.SeedAsync(context);
            await RoomAmenitySeeder.SeedAsync(context);
        }
    }
}
