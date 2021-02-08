using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    public class MockRepositoryInMemoryAsync<TInstance> : BookStoreApi.Contracts.IRepositoryAsync<TInstance> where TInstance : class {
        private readonly DbSet<TInstance> _Dataset;
        private readonly Func<TInstance, IComparable> _KeyTaker;
        private readonly object locker = new object();

        public MockRepositoryInMemoryAsync(DbSet<TInstance> inMemoryDbSet, Expression<Func<TInstance, IComparable>> idAccessor) {
            _Dataset = inMemoryDbSet;
            _KeyTaker = idAccessor.Compile();

        }

        public async Task<bool> CreateAsync(TInstance entity) {
            return (await _Dataset.AddAsync(entity)).State == EntityState.Added;
        }

        public async Task<bool> DeleteAsync(TInstance entity) {
            bool result ;
            try {
                _Dataset.Remove(entity);
                result = await Task.FromResult(true);
            }
            catch (Exception) {
                throw;
            }
            return result;
        }

        public async Task<TInstance> FindAsync(Expression<Func<TInstance, bool>> predicate, IEnumerable<Expression<Func<TInstance, object>>> includes = null) {
            return await Task.Run(() => {
                IQueryable<TInstance> query = _Dataset.Local.AsQueryable();
                return query.FirstOrDefault(predicate);
            });
            
        }

        public async Task<bool> UpdateAsync(TInstance entity) => await Task.FromResult(_Dataset.Update(entity).State == EntityState.Modified);

        public async Task<ICollection<TInstance>> WhereAsync(Expression<Func<TInstance, bool>> filter = null, Func<IQueryable<TInstance>, IOrderedQueryable<TInstance>> order = null, IEnumerable<Expression<Func<TInstance, object>>> includes = null) {
            return await Task.Factory.StartNew<ICollection<TInstance>>(() => {
                IQueryable<TInstance> instances = _Dataset.Local.AsQueryable();
                if(filter != null)
                    instances = instances.Where(filter).AsQueryable();
                return instances.ToList();
            });
        }
    }
}
