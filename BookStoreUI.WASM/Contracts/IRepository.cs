using BookStoreUI.WASM.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Contracts {
    public interface IRepository<TEntity, TKey> 
        where TEntity : class
        where TKey : IComparable<TKey>{
        Task<IEnumerable<TEntity>> Get();

        Task<TEntity> Get(TKey key);

        Task<RepositoryResponce> Create(TEntity entity);

        Task<RepositoryResponce> Update(TEntity entity);

        Task<RepositoryResponce> Delete(TKey key);

    }
}
