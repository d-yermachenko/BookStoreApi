using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStoreApiTests.TestServers {
    public interface ITestClientFactoryAsync {
        Task<HttpClient> GetTestClientAsync(Func<HttpClient, Task> loginActionAsync);
    }
}