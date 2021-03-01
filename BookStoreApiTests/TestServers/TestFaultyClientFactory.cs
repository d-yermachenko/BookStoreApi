using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.TestServers {
    public class TestFaultyClientFactory<TDataSeeder> : TestserverClientFactory, ITestClientFactoryAsync
        where TDataSeeder : class, IAppDataSeeder{

        public TestFaultyClientFactory() : base() {

        }




        protected override TestServer CreateServer() {
            return new TestServer(new WebHostBuilder()
                                     .UseStartup<BookStoreApplication.Startup>()
                                     .UseConfiguration(ConfigurationProvider.BuildBearerConfiguration())
                                     .ConfigureTestServices((services) => {
                                         services.AddSingleton(provider => {
                                             var bookStoreIdentityDbOptions = new DbContextOptionsBuilder<BookStoreIdentityDbContext>()
                                            .UseInMemoryDatabase("IMBookStoreIdentity")
                                            .Options;
                                             var bookStoreIdentityDbContext = new BookStoreIdentityDbContext(bookStoreIdentityDbOptions);
                                             bookStoreIdentityDbContext.Database.EnsureDeleted();
                                             return bookStoreIdentityDbContext;
                                         });
                                         services.AddScoped<IBookStoreUnitOfWorkAsync, Mocks.MockBookStoreFaultyUnitOfWork>();
                                         services.AddTransient<IAppDataSeeder, TDataSeeder>();
                                       }));
        }
    }
}
