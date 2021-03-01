using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUITests.Fakes {
    public class FakeHttpRequestMessageProviderAsync : BookStoreUI.Contracts.IHttpRequestMessageProviderAsync {
        public Task<HttpRequestMessage> CreateMessageAsync(HttpMethod method, string url) {
            return Task.FromResult(new HttpRequestMessage(method, url));
        }
    }


}
