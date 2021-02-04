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
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApiTests.TestServers {
    public class TestFilledClientFactory : ITestClientFactory {

        private readonly TestServer _server;
        private readonly HttpClient _client;
        public TestFilledClientFactory() {
            _server = new TestServer(new WebHostBuilder()
                                     .UseStartup<BookStoreApplication.Startup>()
                                     .UseConfiguration(ConfigurationProvider.BuildConfiguration())
                                     .ConfigureTestServices((services) => {
                                         services.AddSingleton(provider => {
                                             var bookStoreIdentityDbOptions = new DbContextOptionsBuilder<BookStoreIdentityDbContext>()
                                            .UseInMemoryDatabase("IMBookStoreIdentity")
                                            .Options;
                                             var bookStoreIdentityDbContext = new BookStoreIdentityDbContext(bookStoreIdentityDbOptions);
                                             bookStoreIdentityDbContext.Database.EnsureDeleted();
                                             return bookStoreIdentityDbContext;
                                         });

                                         services.AddScoped<IBookStoreUnitOfWorkAsync, MockBookStoreFilledUnitOfWork>();
                                         services.AddControllers().AddNewtonsoftJson(options => {
                                             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                                             options.SerializerSettings.MaxDepth = 2;
                                         });
                                     })) ;
            _client = _server.CreateClient();
        }


        public HttpClient TestClient {
            get => _client;
        }
    }
}
