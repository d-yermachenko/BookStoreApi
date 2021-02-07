using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class EmptyDataSeeder : IAppDataSeeder {

        public EmptyDataSeeder() {

        }



        public async Task SeedData() {
            await Task.CompletedTask;
        }
    }
}
