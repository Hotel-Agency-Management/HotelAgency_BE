using Booking.Constants;
using Microsoft.AspNetCore.Identity;

namespace Booking.Data.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Roles.All)
            {
                var exists = await roleManager.RoleExistsAsync(roleName);

                if (!exists)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to seed role '{roleName}': {errors}");
                    }
                }
            }
        }
    }
}
