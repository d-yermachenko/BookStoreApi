using BookStoreApi.Contracts;
using BookStoreApi.Data.Authentification;
using BookStoreApi.Data.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code {
    public class AppDataSeeder : IAppDataSeeder {

        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<AppRole> _RoleManager;
        private readonly ILogger<IAppDataSeeder> _Logger;

        public AppDataSeeder( 
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ILogger<IAppDataSeeder> logger) {
            _UserManager = userManager;
            _RoleManager = roleManager;
            _Logger = logger;
        }

        public async Task SeedData() {
            _Logger.LogDebug("Seeding data");
            await SeedRoles(_RoleManager);
            await SeedUsers(_UserManager);
            _Logger.LogDebug("Data seed finished");
        }


        private async Task SeedUsers(UserManager<AppUser> userManager) {
            _Logger.LogDebug("Seeding users");
            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null) {
                var admin = AdminDto;
                var user = new AppUser {
                    UserName = admin.Login,
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, admin.Password);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("customer1@gmail.com") == null) {
                var customer = CustomerDto;
                var user = new AppUser {
                    UserName = customer.Login,
                    Email = "customer1@gmail.com"
                };
                var result = await userManager.CreateAsync(user, customer.Password);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("customer2@gmail.com") == null) {
                var user = new AppUser {
                    UserName = "customer2",
                    Email = "customer2@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword512");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private async Task SeedRoles(RoleManager<AppRole> roleManager) {
            _Logger.LogDebug("Seeding roles");
            if (!await roleManager.RoleExistsAsync("Administrator")) {
                var role = new AppRole {
                    Name = "Administrator"
                };
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("Customer")) {
                var role = new AppRole {
                    Name = "Customer"
                };
                await roleManager.CreateAsync(role);
            }
        }

        public static UserLoginDTO AdminDto =>  new UserLoginDTO() {
            Login = "admin",
            Password = "P@ssword1"

        };

        public static UserLoginDTO CustomerDto =>  new UserLoginDTO() {
            Login = "customer1",
            Password = "P@ssword256"
        };
    }
}
