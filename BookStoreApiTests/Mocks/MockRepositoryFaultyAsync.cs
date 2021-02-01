using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockRepositoryFaultyAsync<TEntity> : BookStoreApi.Contracts.IRepositoryAsync<TEntity> where TEntity: class {
        public async Task<bool> CreateAsync(TEntity entity) {
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteAsync(TEntity entity) {
            return await Task.FromResult(false);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            throw new InvalidOperationException("Due the nature of repository, operation failed");
        }

        public async Task<bool> UpdateAsync(TEntity entity) {
            return await Task.FromResult(false);
        }

        public Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            throw new InvalidOperationException("Due the nature of repository, operation failed");
        }
    }
}
