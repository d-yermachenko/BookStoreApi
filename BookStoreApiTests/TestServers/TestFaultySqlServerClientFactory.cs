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
    public class TestFaultySqlServerClientFactory : ITestClientFactory
         {

        private readonly TestServer _server;
        private readonly HttpClient _client;
        public TestFaultySqlServerClientFactory() {
            _server = new TestServer(new WebHostBuilder()
                                     .UseStartup<BookStoreApplication.Startup>()
                                     .UseConfiguration(ConfigurationProvider.BuildConfiguration())
                                     .ConfigureTestServices((services) => {
                                         //https://github.com/aspnet/Hosting/issues/1012

                                         services.AddSingleton(provider => {
                                             var bookStoreDbOptions = new DbContextOptionsBuilder<BookStoreContext>()
                                            .UseSqlServer("Data Source=None\\none;Initial Catalog=none;")
                                            .Options;
                                             var bookStoreDbContext = new MockBookStoreInMemoryContext(bookStoreDbOptions);
                                             bookStoreDbContext.Database.EnsureDeleted();
                                             return bookStoreDbContext;
                                         });

                                         services.AddSingleton(provider => {
                                             var bookStoreIdentityDbOptions = new DbContextOptionsBuilder<BookStoreIdentityDbContext>()
                                            .UseSqlServer("Data Source=None\\none;Initial Catalog=none;")
                                            .Options;
                                             var bookStoreIdentityDbContext = new BookStoreIdentityDbContext(bookStoreIdentityDbOptions);
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
            _client = _server.CreateClient();
        }


        public HttpClient TestClient {
            get => _client;
        }
    }
}
