using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
    public class TestFaultyClientFactory : ITestClientFactory {

        private readonly TestServer _server;
        private readonly HttpClient _client;
        public TestFaultyClientFactory() {
            _server = new TestServer(new WebHostBuilder()
                                     .UseStartup<BookStoreApplication.Startup>()
                                     .UseConfiguration(ConfigurationProvider.BuildConfiguration())
                                     .ConfigureTestServices((services) => {
                                         services.AddScoped<IBookStoreUnitOfWorkAsync, Mocks.MockBookStoreFaultyUnitOfWork>();
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
