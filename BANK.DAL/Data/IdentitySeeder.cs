using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BANK.DAL.Entity;

namespace BANK.DAL.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<
                    RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<
                    UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Admin",
                "Manager",
                "User"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            await CreateUser(
                userManager,
                "admin",
                "Admin123!",
                "Admin");

            await CreateUser(
                userManager,
                "manager",
                "Manager123!",
                "Manager");

            await CreateUser(
                userManager,
                "user",
                "User123!",
                "User");
        }

        private static async Task CreateUser(
            UserManager<ApplicationUser> userManager,
            string username,
            string password,
            string role)
        {
            var user =
                await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username
                };

                var result =
                    await userManager.CreateAsync(
                        user,
                        password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(
                            ", ",
                            result.Errors.Select(
                                x => x.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(
                    user,
                    role))
            {
                await userManager.AddToRoleAsync(
                    user,
                    role);
            }
        }
    }
}
