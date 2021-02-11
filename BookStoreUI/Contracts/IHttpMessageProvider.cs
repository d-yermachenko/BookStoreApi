using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStoreUI.Contracts {
    public interface IHttpRequestMessageProviderAsync {
        Task<HttpRequestMessage> CreateMessageAsync(HttpMethod method, string url);
    }
}
