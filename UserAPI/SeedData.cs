using Microsoft.AspNetCore.Identity;
using UserAPI.Models;

namespace UserAPI
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create role if they don't exist
            var roleExists = await roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create admin users
            for (int i = 1; i <= 2; i++)
            {
                var adminUser = new User
                {
                    UserName = $"admin{i}@example.com",
                    Email = $"admin{i}@example.com",
                    Name = $"Admin User {i}",
                    PhoneNumber = $"987654321{i}"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create regular users
            for (int i = 1; i <= 3; i++)
            {
                var regularUser = new User
                {
                    UserName = $"user{i}@example.com",
                    Email = $"user{i}@example.com",
                    Name = $"Regular User {i}",
                    PhoneNumber = $"987654321{9-i}"
                };
                await userManager.CreateAsync(regularUser, "User@123");
            }
        }
    }
}
