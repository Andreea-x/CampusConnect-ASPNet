using CampusConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // Seed the required application roles before any user assignment occurs.
        var roles = new[] { "Admin", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminEmail = "admin@campusconnect.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Campus Administrator",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed a small set of starter events so the coursework app has usable data on first run.
        if (!await context.Events.AnyAsync())
        {
            context.Events.AddRange(
                new Event
                {
                    Title = "Orientation Connect",
                    Description = "Welcome session for new students with clubs, mentors, and campus services.",
                    EventDate = DateTime.UtcNow.AddDays(7),
                    Capacity = 150
                },
                new Event
                {
                    Title = "Career Skills Workshop",
                    Description = "Interactive workshop focused on CV writing, networking, and interview practice.",
                    EventDate = DateTime.UtcNow.AddDays(14),
                    Capacity = 80
                });

            await context.SaveChangesAsync();
        }
    }
}
