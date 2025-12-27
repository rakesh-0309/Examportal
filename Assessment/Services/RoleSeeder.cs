using Assessment.Model;
//using ExamPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace Assessment;

public static class RoleSeeder
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public static async Task EnsureSeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, string adminEmail, string adminPassword)
    {
        if (!await roleManager.RoleExistsAsync(AdminRole))
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        if (!await roleManager.RoleExistsAsync(UserRole))
            await roleManager.CreateAsync(new IdentityRole(UserRole));

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FullName = "System Admin" };
            var res = await userManager.CreateAsync(admin, adminPassword);
            if (res.Succeeded)
                await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}
