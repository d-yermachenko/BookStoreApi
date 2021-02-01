using System.Net.Http;

namespace BookStoreApiTests.TestServers {
    public interface ITestClientFactory {
        HttpClient TestClient { get; }
    }
}