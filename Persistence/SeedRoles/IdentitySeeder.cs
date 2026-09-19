using exam_system.Common.Enums;
using exam_system.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Domain.Entities.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // seed role 
            if (!await roleManager.RoleExistsAsync(SD.Admin))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(SD.Admin));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException($"Unable to create admin role : {errors}");
                }
            }
            if (!await roleManager.RoleExistsAsync(SD.Student))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(SD.Student));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException($"Unable to create student role : {errors}");
                }
            }

            // create admin account 
            var adminEmail = "admin@team4.net";
            var adminPassword = "Admin12c#";
            var adminFullName = "super admin";

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException("Admin seed credentials are missing.");
            }
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    FullName = adminFullName,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    AccountStatus = AccountStatus.Active
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException($"Unable to create admin: {errors}");
                }


            }
            if (!await userManager.IsInRoleAsync(admin, SD.Admin))
            {
                var roleResult = await userManager.AddToRoleAsync(admin, SD.Admin);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException($"Unable to assign Admin role: {errors}");
                }
            }


        }


    }


}
