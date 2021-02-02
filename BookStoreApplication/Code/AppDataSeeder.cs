using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code {
    public class AppDataSeeder : IAppDataSeeder {

        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly ILogger<IAppDataSeeder> _Logger;

        public AppDataSeeder(IBookStoreUnitOfWorkAsync bookStore, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IAppDataSeeder> logger) {
            _BookStore = bookStore;
            _UserManager = userManager;
            _RoleManager = roleManager;
            _Logger = logger;
        }

        public async Task SeedData() {

            await SeedRoles(_RoleManager);
            await SeedUsers(_UserManager);
        }


        private async static Task SeedUsers(UserManager<IdentityUser> userManager) {
            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null) {
                var user = new IdentityUser {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("customer1@gmail.com") == null) {
                var user = new IdentityUser {
                    UserName = "customer1",
                    Email = "customer1@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("customer2@gmail.com") == null) {
                var user = new IdentityUser {
                    UserName = "customer2",
                    Email = "customer2@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager) {
            if (!await roleManager.RoleExistsAsync("Administrator")) {
                var role = new IdentityRole {
                    Name = "Administrator"
                };
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Customer")) {
                var role = new IdentityRole {
                    Name = "Customer"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
