using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.Data.Authentification;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockDataSeeder : IAppDataSeeder {

        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<AppRole> _RoleManager;
        public MockDataSeeder(IBookStoreUnitOfWorkAsync bookStore,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager) {
            _BookStore = bookStore;
            _UserManager = userManager;
            _RoleManager = roleManager;
        }

        private async Task SeedBooksAndAuthors() {
            Author aStugatsky = new Author() {
                Id = 1,
                Firstname = "Arkady",
                Lastname = "Strugatski",
                Bio = "He was exelent writer"
            };

            Author bStugatsky = new Author() {
                Id = 2,
                Firstname = "Boris",
                Lastname = "Strugatski",
                Bio = "He was exelent writer and scientist"
            };

            Author iAzymov = new Author() {
                Id = 3,
                Firstname = "Isaak",
                Lastname = "Azimov",
                Bio = "He is excellent writer and scientis"
            };

            Book mondayStartsSaturday = new Book() {
                Id = 1,
                Title = "Monday starts saturday",
                Year = 1960,
                Isbn = "978-5-17-106224-8",
                Summary = "Exelent partody to scientists community",
                Authors = new Author[] { aStugatsky, bStugatsky }
            };

            Book hardToBeTheGod = new Book() {
                Id = 2,
                Title = "Hard to be the God",
                Year = 1960,
                Isbn = "978-5-17-106224-8",
                Summary = "Another view what happens to civilizations",
                Authors = new Author[] { aStugatsky, bStugatsky }
            };

            Book theGodsThemselves = new Book() {
                Id = 3,
                Title = "The Gods Themselves",
                Year = 1972,
                Isbn = "978-8-49-800851-7",
                Summary = "Another view what happens to civilizations",
                Authors = new Author[] { iAzymov }
            };
            aStugatsky.Books = new Book[] { mondayStartsSaturday, hardToBeTheGod };
            bStugatsky.Books = new Book[] { mondayStartsSaturday, hardToBeTheGod };
            iAzymov.Books = new Book[] { theGodsThemselves };
            await _BookStore.Authors.CreateAsync(aStugatsky);
            await _BookStore.Authors.CreateAsync(bStugatsky);
            await _BookStore.Authors.CreateAsync(iAzymov);
            await _BookStore.Books.CreateAsync(mondayStartsSaturday);
            await _BookStore.Books.CreateAsync(hardToBeTheGod);
            await _BookStore.Books.CreateAsync(theGodsThemselves);
        }

        private async static Task SeedUsers(UserManager<AppUser> userManager) {
            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null) {
                var user = new AppUser {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword128!");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("customer1@gmail.com") == null) {
                var user = new AppUser {
                    UserName = "customer1",
                    Email = "customer1@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword2");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("customer2@gmail.com") == null) {
                var user = new AppUser {
                    UserName = "customer2",
                    Email = "customer2@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword3");
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

        public async Task SeedData() {
            await SeedRoles(_RoleManager);
            await SeedUsers(_UserManager);
            await SeedBooksAndAuthors();
        }
    }
}
