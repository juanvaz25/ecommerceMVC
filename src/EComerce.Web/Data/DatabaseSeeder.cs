using EComerce.Domain.Enums;
using EComerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EComerce.Web.Data;

/// <summary>
/// Siembra los roles de la aplicación y un usuario administrador inicial.
/// Se ejecuta una sola vez al arrancar en Development si no existen los datos.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        await SeedRolesAsync(roleManager, logger);
        await SeedAdminUserAsync(userManager, logger);
    }

    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        var roles = new[] { nameof(UserRole.Admin), nameof(UserRole.Customer) };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                    logger.LogInformation("Rol '{Role}' creado.", role);
                else
                    logger.LogWarning("Error creando rol '{Role}': {Errors}", role, result.Errors);
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        const string adminEmail = "admin@ecomerce.local";

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null) return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "System",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@1234!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, nameof(UserRole.Admin));
            logger.LogInformation("Usuario administrador inicial creado: {Email}", adminEmail);
        }
        else
        {
            logger.LogWarning("Error creando admin: {Errors}", result.Errors);
        }
    }
}
