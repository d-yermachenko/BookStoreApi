using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using BookStoreUI.WASM.Data.Models;
using BookStoreUI.WASM.Contracts;

namespace BookStoreUI.WASM.Services {
    public abstract class RepositoryBase<TEntity,  TKey> : IRepository<TEntity,  TKey>
        where TEntity : class
        where TKey : IComparable<TKey> {

        private readonly string _BaseUrl;
        private readonly IServiceRepository<TEntity, TKey> _ServiceRepository;
        protected abstract string ActionPath { get; }

        protected RepositoryBase(
            IConfiguration configuration,
            HttpClient httpClientFactory,
            IHttpRequestMessageProviderAsync messageProvider,
            Expression<Func<TEntity, TKey>> keyTaker) {
            _BaseUrl = configuration.GetValue<string>("ConnectionStrings:DefaultUrl");
            _ServiceRepository = new ServiceRepositoryBase<TEntity, TKey>(httpClientFactory, keyTaker, messageProvider);
        }

        protected string CombineUrl() => Url.Combine(_BaseUrl, ActionPath);

        public async Task<RepositoryResponce> Create(TEntity entity) => await _ServiceRepository.Create(CombineUrl(), entity);

        public async Task<RepositoryResponce> Delete(TKey key) => await _ServiceRepository.Delete(CombineUrl(), key);

        public async Task<IEnumerable<TEntity>> Get() => await _ServiceRepository.Get(CombineUrl());

        public async Task<TEntity> Get(TKey key) => await _ServiceRepository.Get(CombineUrl(), key);

        public async Task<RepositoryResponce> Update(TEntity entity) => await _ServiceRepository.Update(CombineUrl(), entity);

    }
}
