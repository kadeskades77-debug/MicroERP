
using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using Microsoft.AspNetCore.Identity;


namespace MicroERP.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        string[] roles =
        {
            "SuperAdmin",
            "Admin",
            "Manager",
            "HR",
            "Accountant",
            "StoreKeeper",
            "Sales",
            "Administration",
            "Employee"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role,
                    IsSystem = true
                });
            }
        }
        //==============================
        const string email = "admin@microerp.com";

        var admin =
     await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                FullName = "System Administrator",
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(
                admin,
                "Admin@123");
            var roleResult = await userManager.AddToRoleAsync(admin, SystemRoles.SuperAdmin);

        }

        if (!await userManager.IsInRoleAsync(
                admin,
                "SuperAdmin"))
        {
            await userManager.AddToRoleAsync(
                admin,
                "SuperAdmin");
        }
    }
    
}