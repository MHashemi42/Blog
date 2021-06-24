using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager, configuration);
        }

        private static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("Admin").Result is false)
            {
                ApplicationRole role = new() { Name = "Admin" };
                roleManager.CreateAsync(role).Wait();
            }
        }

        private static void SeedUsers(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var username = configuration["Admin:Username"];
            var friendlyName = configuration["Admin:FriendlyName"];
            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];

            if (userManager.FindByNameAsync(username).Result is null)
            {
                ApplicationUser user = new()
                {
                    UserName = username,
                    FriendlyName = friendlyName, 
                    Email = email,
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
