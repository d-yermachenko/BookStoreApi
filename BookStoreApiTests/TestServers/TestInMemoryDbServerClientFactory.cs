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
using BookStoreApiTests.Mocks.Users;

namespace BookStoreApiTests.TestServers {
    public class TestInMemoryDbServerClientFactory : ITestClientFactory {

        private readonly TestServer _server;
        private readonly HttpClient _client;
        public TestInMemoryDbServerClientFactory() {
            _server = new TestServer(new WebHostBuilder()
                                     .UseStartup<BookStoreApplication.Startup>()
                                     .UseConfiguration(ConfigurationProvider.BuildConfiguration())
                                     .ConfigureTestServices((services) => {
                                         services.AddDbContext<BookStoreContext>(options => {
                                             options.UseInMemoryDatabase(databaseName: "IMBookStore");
                                         });
                                         services.AddDbContext<BookStoreIdentityDbContext>(options => {
                                             options.UseInMemoryDatabase(databaseName: "IMBookStoreIdentity");
                                         });
                                         services.AddTransient<IAppDataSeeder, MockDataSeeder>();
                                         services.AddScoped<IBookStoreUnitOfWorkAsync, MockBookStoreUsersEnanledUnitOfWork>();
                                         services.AddControllers().AddNewtonsoftJson(options => {
                                             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                                             options.SerializerSettings.MaxDepth = 2;
                                         });
                                     })
                                            );
            _client = _server.CreateClient();
        }


        public HttpClient TestClient {
            get => _client;
        }
    }
}
