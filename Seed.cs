using MyGameStoreModel.Entities;
using Microsoft.AspNetCore.Identity;


namespace MyGameStore;

public class Seed
{
    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
        string adminUserEmail = "n89190245729@gmail.com";

        var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
        if (adminUser == null)
        {
            var newAdminUser = new User()
            {
                UserName = "ne_nikita",
                Email = adminUserEmail,
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(newAdminUser, "Vistion@1992?");
            await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
        }


        string appUserEmail = "user@etickets.com";

        var appUser = await userManager.FindByEmailAsync(appUserEmail);
        if (appUser == null)
        {
            var newAppUser = new User()
            {
                UserName = "app-user",
                Email = appUserEmail,
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(newAppUser, "Vistion@1992?");
            await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
        }
    }
}