using Microsoft.AspNetCore.Identity;

namespace AppService.Data;

public class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Seller", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await CreateUserAsync(userManager, "seller@test.com", "Password123!", "Seller");
        await CreateUserAsync(userManager, "user@test.com", "Password123!", "User");
    }

    private static async Task CreateUserAsync(
        UserManager<IdentityUser> userManager, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) is null)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
