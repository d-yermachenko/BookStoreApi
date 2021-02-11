using BookStoreUI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Contracts {
    public interface IServiceRepository<TEntity, TKey> 
        where TEntity : class
        where TKey :  IComparable<TKey>{
        Task<IEnumerable<TEntity>> Get(string url);

        Task<TEntity> Get(string url, TKey key);

        Task<RepositoryResponce> Create(string url, TEntity entity);

        Task<RepositoryResponce> Update(string url, TEntity entity);

        Task<RepositoryResponce> Delete(string url, TKey key);

    }
}
