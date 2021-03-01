using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockRepositoryFaultyAsync<TEntity, TKey> : BookStoreApi.Contracts.IRepositoryAsync<TEntity, TKey> 
        where TEntity: class
        where TKey: IComparable<TKey>{
        public Task<bool> CreateAsync(TEntity entity) {
            throw new InvalidOperationException("Due the nature of repository, create operation failed");
        }

        public Task<bool> DeleteAsync(TKey entity) {
            throw new InvalidOperationException("Due the nature of repository, delete operation failed");
        }

        public Task<TEntity> FindAsync(TKey id, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            throw new InvalidOperationException("Due the nature of repository, find operation failed");
        }


        public Task<bool> UpdateAsync(TEntity entity) {
            throw new InvalidOperationException("Due the nature of repository, update operation failed");
        }

        public Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            throw new InvalidOperationException("Due the nature of repository, read all failed");
        }
    }
}
