using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts {
    public interface IRepositoryAsync<TEntity, TKey> where TEntity: class
        where TKey: IComparable<TKey>
        {
        Task<bool> CreateAsync(TEntity entity);

        Task<bool> DeleteAsync(TKey id);

        Task<TEntity> FindAsync(TKey id, IEnumerable<Expression<Func<TEntity, object>>> includes = null);

        Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null, IEnumerable<Expression<Func<TEntity, object>>> includes = null);

        Task<bool> UpdateAsync(TEntity entity);
    }
}
