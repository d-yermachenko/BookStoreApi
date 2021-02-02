using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockDataSeeder : BookStoreApi.Contracts.IAppDataSeeder {

        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        public MockDataSeeder(IBookStoreUnitOfWorkAsync bookStore,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager) {
            _BookStore = bookStore;
            _UserManager = userManager;
            _RoleManager = roleManager;
        }

        public async Task SeedData() {
            await _BookStore.Authors.CreateAsync(new BookStoreApi.Data.Author() {
                Id= 1,
                Firstname = "Ray",
                Lastname = "Bradbury"
            });
            await _BookStore.Authors.CreateAsync(new BookStoreApi.Data.Author() {
                Id = 2,
                Firstname = "Agatha",
                Lastname = "Cristy"
            });
        }
    }
}
