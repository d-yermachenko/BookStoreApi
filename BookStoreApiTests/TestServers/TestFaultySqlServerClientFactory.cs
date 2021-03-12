using BookStoreApi.Code.DataContoroller.Entity;
using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using BookStoreApiTests.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using BookStoreApi.Data;
using BookStoreApiTests.Mocks.InMemory;

namespace BookStoreApiTests.TestServers {
    public class TestFaultySqlServerClientFactory : TestserverClientFactory, ITestClientFactoryAsync
         {

        public TestFaultySqlServerClientFactory() : base(){
  
        }


        protected override TestServer CreateServer() {
            var config = ConfigurationProvider.BuildConfiguration();
            return new TestServer(new WebHostBuilder()
                                     .UseStartup<BookStoreApplication.Startup>()
                                     .UseConfiguration(config)
                                     .ConfigureTestServices((services) => {
                                         //https://github.com/aspnet/Hosting/issues/1012

                                         services.AddSingleton(provider => {
                                             var bookStoreDbOptions = new DbContextOptionsBuilder<BookStoreContext>()
                                            .UseSqlServer("Data Source=None\\none;Initial Catalog=none;")
                                            .Options;
                                             var bookStoreDbContext = new MockBookStoreInMemoryContext(bookStoreDbOptions, config);
                                             bookStoreDbContext.Database.EnsureDeleted();
                                             return bookStoreDbContext;
                                         });

                                         services.AddSingleton(provider => {
                                             var bookStoreIdentityDbOptions = new DbContextOptionsBuilder<BookStoreIdentityDbContext>()
                                            .UseSqlServer("Data Source=None\\none;Initial Catalog=none;")
                                            .Options;
                                             var bookStoreIdentityDbContext = new BookStoreIdentityDbContext(bookStoreIdentityDbOptions, config);
                                             bookStoreIdentityDbContext.Database.EnsureDeleted();
                                             return bookStoreIdentityDbContext;
                                         });

                                         services.AddScoped<IBookStoreUnitOfWorkAsync, MockBookStoreInMemoryUnitOfWork>();
                                         services.AddTransient<IAppDataSeeder, EmptyDataSeeder>();

                                         services.AddControllers().AddNewtonsoftJson(options => {
                                             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                                             options.SerializerSettings.MaxDepth = 2;
                                         });
                                     }));
        }
    }
}
