using Blazored.LocalStorage;
using BookStoreUI.Contracts;
using BookStoreUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStoreUI.Services {
    public class HttpRequestMessageProvider : IHttpRequestMessageProviderAsync {

        private readonly ILocalStorageService _LocalStorage;

        public HttpRequestMessageProvider(ILocalStorageService localStorageService = null) {
            _LocalStorage = localStorageService;
        }

        public async Task<HttpRequestMessage> CreateMessageAsync(HttpMethod method, string url) {
            var message = new HttpRequestMessage(method, url);
            if (_LocalStorage != null) {
                string token = await _LocalStorage.GetItemAsStringAsync(ConventionalKeys.TokenStorageKey);
                if (!String.IsNullOrWhiteSpace(token))

                    message.Headers.Add("Bearer", token);
            }
            return message;
        }
    }
}
