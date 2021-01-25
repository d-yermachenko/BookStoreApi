using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts {
    public interface IRepositoryAsync<TEntity> where TEntity: class {
        Task<bool> CreateAsync(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes = null);

        Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null, IEnumerable<Expression<Func<TEntity, object>>> includes = null);


    }
}
