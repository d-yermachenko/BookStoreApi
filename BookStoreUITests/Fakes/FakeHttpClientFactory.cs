using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUITests.Fakes {
    public class FakeHttpClientFactory : IHttpClientFactory {
        public HttpClient CreateClient(string name) {
            return new BookStoreApiTests.TestServers.TestInMemoryDbServerClientFactory<
                BookStoreApiTests.Mocks.MockDataSeeder>().TestClient;
        }
    }
}
