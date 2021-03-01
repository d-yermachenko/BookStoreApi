using BookStoreApi.Contracts;
using BookStoreApi.Data.DTOs;
using BookStoreApiTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUITests.Fakes {
    public class FakeInMemoryFaultyHttpClientFactory<TDataSeeder> : IHttpClientFactory
        where TDataSeeder: class, IAppDataSeeder {

        private readonly UserLoginDTO _LoginDTO; 

        public FakeInMemoryFaultyHttpClientFactory(UserLoginDTO login = null) {
            _LoginDTO = login;
            _ClientFactory = new BookStoreApiTests.TestServers.TestFaultyClientFactory<TDataSeeder>();
        }

        private readonly BookStoreApiTests.TestServers.ITestClientFactoryAsync _ClientFactory;
        public HttpClient CreateClient(string name) {
            Task<HttpClient> client;
            if (_LoginDTO != null)
                client = _ClientFactory.GetTestClientAsync((client) =>
                BookStoreApiTests.TestServers.AuthorizeMethods.Autorize(
                        () => _LoginDTO, client));
            else
                client = _ClientFactory.GetTestClientAsync(null);
            return client.Result;
        }
    }
}