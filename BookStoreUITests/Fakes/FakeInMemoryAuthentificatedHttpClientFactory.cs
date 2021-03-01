using BookStoreApi.Code;
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
    public class FakeInMemoryAuthentificatedHttpClientFactory<TDataSeeder> : IHttpClientFactory
        where TDataSeeder: class, IAppDataSeeder{



        public FakeInMemoryAuthentificatedHttpClientFactory(UserLoginDTO login = null) {
            _ClientFactory = new BookStoreApiTests.TestServers.TestInMemoryAuthentificatedDbServerClientFactory<TDataSeeder>(() => login);

        }

        private readonly BookStoreApiTests.TestServers.ITestClientFactoryAsync  _ClientFactory;
        public HttpClient CreateClient(string name) {
            return  _ClientFactory.GetTestClientAsync(null).Result;
        }
    }
}