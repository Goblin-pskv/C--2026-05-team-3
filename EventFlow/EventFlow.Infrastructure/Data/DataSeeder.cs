using EventFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EventFlow.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager,
                                                UserManager<User> userManager)
        {
            string[] roleNames = { "Admin", "User", "Organizer" };

            foreach (var roleName in roleNames)
            {
                bool roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    var role = new IdentityRole<Guid>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    };
                    await roleManager.CreateAsync(role);

                }
            }

            var adminEmail = "admin@eventflow.com";
            var adminPassword = "DevAdmin@123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(newAdmin, "Admin");

                    if (addToRoleResult.Succeeded)
                    {
                        Console.WriteLine($"Роль Admin назначена пользователю {adminEmail}");
                        Console.WriteLine($"Логин: {adminEmail}");
                        Console.WriteLine($"Пароль: {adminPassword}");
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка при назначении роли: " +
                            $"{string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }

                else
                {
                    Console.WriteLine($"Ошибка создания пользователя: " +
                        $"{string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            else
            {
                // логируем что админ уже существует.
            }

        }
    }
}
