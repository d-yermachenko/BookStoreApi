using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Mime;
using System.Text;
using BookStoreUI.WASM.Data.Models;
using BookStoreUI.WASM.Contracts;
using System.Net.Http.Json;

namespace BookStoreUI.WASM.Services {
    public class ServiceRepositoryBase<TEntity,  TKey> : IServiceRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IComparable<TKey> {


        private readonly HttpClient _Client;
        private readonly Func<TEntity, TKey> _KeyTaker;
        private readonly IHttpRequestMessageProviderAsync _RequestMessageProvider;

        public ServiceRepositoryBase(HttpClient httpClient,
            Expression<Func<TEntity, TKey>> keyTaker,
            IHttpRequestMessageProviderAsync messageProvider) {
            _Client = httpClient;
            _KeyTaker = keyTaker.Compile();
            _RequestMessageProvider = messageProvider;

        }
        public async Task<RepositoryResponce> Create(string url, TEntity entity) {
            if (entity == null)
                return RepositoryResponce.ArgumentNullResponce;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, MediaTypeNames.Application.Json);
            var responce = await _Client.SendAsync(request);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }

        public async Task<RepositoryResponce> Delete(string url, TKey key) {
            if (key == null)
                return RepositoryResponce.ArgumentNullResponce;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Delete, Flurl.Url.Combine(url, key.ToString()));
            var responce = await _Client.SendAsync(request);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }

        public async Task<IEnumerable<TEntity>> Get(string url) {
            if (string.IsNullOrWhiteSpace(url))
                return Array.Empty<TEntity>();
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Get, url);
            var responce = await _Client.SendAsync(request);
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
                return Array.Empty<TEntity>();

            var jsonContent = await responce.Content.ReadAsStringAsync();
            //return Array.Empty<TEntity>();
            return JsonConvert.DeserializeObject<IEnumerable<TEntity>>(jsonContent
                , new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        public async Task<TEntity> Get(string url, TKey key) {
            if (string.IsNullOrWhiteSpace(url))
                return default;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Get, Flurl.Url.Combine(url, key.ToString()));
            var responce = await _Client.SendAsync(request);
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
                return default;
            return JsonConvert.DeserializeObject<TEntity>(await responce.Content.ReadAsStringAsync());
        }

        public async Task<RepositoryResponce> Update(string url, TEntity entity) {
            if (entity == null)
                return RepositoryResponce.ArgumentNullResponce;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Put, Flurl.Url.Combine(url, _KeyTaker.Invoke(entity).ToString()));
            string entityJSON = JsonConvert.SerializeObject(entity, new JsonSerializerSettings() {
                MaxDepth = 1,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            request.Content = new StringContent(entityJSON, Encoding.UTF8, MediaTypeNames.Application.Json);
            var responce = await _Client.SendAsync(request);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }
    }
}
