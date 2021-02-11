using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.Data.Authentification;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code {

    /// <summary>
    /// 
    /// </summary>
    /// <deprecated/>
    public class AppInMemoryDataSeeder : IAppDataSeeder {

        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<AppRole> _RoleManager;
        private readonly ILogger<IAppDataSeeder> _Logger;

        public AppInMemoryDataSeeder(IBookStoreUnitOfWorkAsync bookStore, 
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ILogger<IAppDataSeeder> logger) {
            _BookStore = bookStore;
            _UserManager = userManager;
            _RoleManager = roleManager;
            _Logger = logger;
        }



        public async Task SeedData() {

            await SeedAuthors();
            await SeedBooks();
            await SeedRoles(_RoleManager);
            await SeedUsers(_UserManager);
        }


        private async static Task SeedUsers(UserManager<AppUser> userManager) {
            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null) {
                var user = new AppUser {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("customer1@gmail.com") == null) {
                var user = new AppUser {
                    UserName = "customer1",
                    Email = "customer1@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("customer2@gmail.com") == null) {
                var user = new AppUser {
                    UserName = "customer2",
                    Email = "customer2@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private async static Task SeedRoles(RoleManager<AppRole> roleManager) {
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

        private async Task SeedAuthors() {
            await _BookStore.Authors.CreateAsync(new BookStoreApi.Data.Author() {
                Id = 1,
                Firstname = "Ray",
                Lastname = "Bradbury"
            });
            await _BookStore.Authors.CreateAsync(new BookStoreApi.Data.Author() {
                Id = 2,
                Firstname = "Agatha",
                Lastname = "Cristy"
            });
        }

        private async Task SeedBooks() {
            Book mondayStartsSaturday = new Book() {
                Id = 1,
                Title = "Monday starts saturday",
                Year = 1960,
                Isbn = "978-5-17-106224-8",
                Summary = "Exelent partody to scientists community",
                Authors = Array.Empty<Author>()
            };

            Book hardToBeTheGod = new Book() {
                Id = 2,
                Title = "Hard to be the God",
                Year = 1960,
                Isbn = "978-5-17-106224-8",
                Summary = "Another view what happens to civilizations",
                Authors = Array.Empty<Author>()
            };

            Book theGodsThemselves = new Book() {
                Id = 3,
                Title = "The Gods Themselves",
                Year = 1972,
                Isbn = "978-8-49-800851-7",
                Summary = "Another view what happens to civilizations",
                Authors = Array.Empty <Author>()
            };

            await _BookStore.Books.CreateAsync(mondayStartsSaturday);
            await _BookStore.Books.CreateAsync(hardToBeTheGod);
            await _BookStore.Books.CreateAsync(theGodsThemselves);
        }

    }
}
