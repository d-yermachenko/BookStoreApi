﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Mime;
using System.Text;
using BookStoreUI.Data.Models;
using Blazored.LocalStorage;
using BookStoreUI.Contracts;

namespace BookStoreUI.Services {
    public class ServiceRepositoryBase<TEntity, TKey> : Contracts.IServiceRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IComparable<TKey> {


        private readonly IHttpClientFactory _ClientFactory;
        private readonly Func<TEntity, TKey> _KeyTaker;
        private readonly IHttpRequestMessageProviderAsync _RequestMessageProvider;

        public ServiceRepositoryBase(IHttpClientFactory clientFactory,
            Expression<Func<TEntity, TKey>> keyTaker,
            IHttpRequestMessageProviderAsync messageProvider) {
            _ClientFactory = clientFactory;
            _KeyTaker = keyTaker.Compile();
            _RequestMessageProvider = messageProvider;

        }
        public async Task<RepositoryResponce> Create(string url, TEntity entity) {
            if (entity == null)
                return RepositoryResponce.ArgumentNullResponce;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Post, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = _ClientFactory.CreateClient();
            var responce = await client.SendAsync(request);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }

        public async Task<RepositoryResponce> Delete(string url, TKey key) {
            if (key == null)
                return RepositoryResponce.ArgumentNullResponce;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Delete, Flurl.Url.Combine(url, key.ToString()));
            var client = _ClientFactory.CreateClient();
            var responce = await client.SendAsync(request);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }

        public async Task<IEnumerable<TEntity>> Get(string url) {
            if (string.IsNullOrWhiteSpace(url))
                return Array.Empty<TEntity>();
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Get, url);
            var client = _ClientFactory.CreateClient();
            var responce = await client.SendAsync(request);
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
                return Array.Empty<TEntity>();
            return JsonConvert.DeserializeObject<IEnumerable<TEntity>>(await responce.Content.ReadAsStringAsync());
        }

        public async Task<TEntity> Get(string url, TKey key) {
            if (string.IsNullOrWhiteSpace(url))
                return default;
            var request = await _RequestMessageProvider.CreateMessageAsync(HttpMethod.Get, Flurl.Url.Combine(url, key.ToString()));
            var client = _ClientFactory.CreateClient();
            var responce = await client.SendAsync(request);
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
            var client = _ClientFactory.CreateClient();
            var responce = await client.SendAsync(request);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }
    }
}
