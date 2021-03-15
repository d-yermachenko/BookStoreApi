using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    public class MockRepositoryInMemoryAsync<TInstance, TKey> : BookStoreApi.Contracts.IRepositoryAsync<TInstance, TKey>
        where TInstance : class
        where TKey : IComparable<TKey> {
        private readonly DbSet<TInstance> _Dataset;
        private readonly Func<TInstance, TInstance> _InstanceCooker;


        public MockRepositoryInMemoryAsync(DbSet<TInstance> inMemoryDbSet, Func<TInstance, TInstance> prepareMethod) {
            _Dataset = inMemoryDbSet;
            _InstanceCooker = prepareMethod;
        }

        public async Task<bool> CreateAsync(TInstance entity) {
            var preparesInstance = _InstanceCooker.Invoke(entity);
            var result = _Dataset.Add(preparesInstance);
            return await Task.FromResult(result.State == EntityState.Added);
        }

        public async Task<bool> DeleteAsync(TKey id) {
            bool result;
            try {
                var entity = await _Dataset.FindAsync(id);
                if (entity == null)
                    throw new KeyNotFoundException($"Element of type {typeof(TInstance).Name} with key {id} was not found");
                _Dataset.Remove(entity);
                result = await Task.FromResult(true);
            }
            catch (Exception) {
                throw;
            }
            return result;
        }



        public async Task<TInstance> FindAsync(Expression<Func<TInstance, bool>> idPredicate, IEnumerable<Expression<Func<TInstance, object>>> includes = null) {
            return await Task.Run(() => {
                IQueryable<TInstance> query = _Dataset.AsQueryable();
                if (includes != null) {
                    foreach (var include in includes) {
                        query = query.Include(include);
                    }
                }
                return query.FirstOrDefault(idPredicate);
            });
        }

        public async Task<bool> UpdateAsync(TInstance entity) => await Task.FromResult(_Dataset.Update(entity).State == EntityState.Modified);

        public async Task<ICollection<TInstance>> WhereAsync(Expression<Func<TInstance, bool>> filter = null, Func<IQueryable<TInstance>, IOrderedQueryable<TInstance>> order = null, IEnumerable<Expression<Func<TInstance, object>>> includes = null) {
            return await Task.Factory.StartNew<ICollection<TInstance>>(() => {
                IQueryable<TInstance> instances = _Dataset.AsQueryable();
                if (filter != null)
                    instances = instances.Where(filter).AsQueryable();
                return instances.ToList();
            });
        }
    }
}
