using AvitalERP.Models;
using Microsoft.AspNetCore.Identity;

namespace AvitalERP.Data
{
    public static class IdentitySeed
    {
        public static readonly string[] Roles = new[]
        {
            "admin", "administrativo", "operacion", "externos"
        };

        public static async Task SeedAsync(IServiceProvider services, string adminEmail, string adminPassword)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            // Roles
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin user
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception("No se pudo crear admin: " + errors);
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "admin"))
                await userManager.AddToRoleAsync(admin, "admin");
        }
    }
}

