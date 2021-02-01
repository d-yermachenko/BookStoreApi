﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockRepositoryAsync<TInstance> : BookStoreApi.Contracts.IRepositoryAsync<TInstance> where TInstance : class {
        private readonly ConcurrentDictionary<IComparable, TInstance> _Dataset;
        private readonly Func<TInstance, IComparable> _KeyTaker;
        private readonly object locker = new object();

        public MockRepositoryAsync(Expression<Func<TInstance, IComparable>> idAccessor) {
            _Dataset = new ConcurrentDictionary<IComparable, TInstance>();
            _KeyTaker = idAccessor.Compile();
        }

        public Task<bool> CreateAsync(TInstance entity) {
            return Task.Factory.StartNew<bool>(() => {
                IComparable id = _KeyTaker.Invoke(entity);
                if (_Dataset.ContainsKey(id))
                    return false;
                else
                    return _Dataset.TryAdd(id, entity);
            });


        }

        public Task<bool> DeleteAsync(TInstance entity) {
            return Task.FromResult(_Dataset.TryRemove(_KeyTaker.Invoke(entity), out _));
        }

        public Task<TInstance> FindAsync(Expression<Func<TInstance, bool>> predicate, IEnumerable<Expression<Func<TInstance, object>>> includes = null) {
            return Task.Factory.StartNew(() => {
                IQueryable<TInstance> instances = _Dataset.Values.AsQueryable();
                var requestedData = instances.Where(predicate);
                return requestedData.FirstOrDefault();
            });
        }

        public Task<bool> UpdateAsync(TInstance entity) {
            return Task.Factory.StartNew(() => {
                IQueryable<TInstance> instances = _Dataset.Values.AsQueryable();
                lock (locker) {
                    IComparable id = _KeyTaker.Invoke(entity);
                    var originalInstance = _Dataset[id];
                    return _Dataset.TryUpdate(id, entity, originalInstance);
                }
            });
        }

        public Task<ICollection<TInstance>> WhereAsync(Expression<Func<TInstance, bool>> filter = null, Func<IQueryable<TInstance>, IOrderedQueryable<TInstance>> order = null, IEnumerable<Expression<Func<TInstance, object>>> includes = null) {
            return Task.Factory.StartNew<ICollection<TInstance>>(() => {
                IQueryable<TInstance> instances = _Dataset.Values.AsQueryable();
                if(filter != null)
                    instances = instances.Where(filter).AsQueryable();
                if(includes?.Count() > 0) {
                    foreach (var include in includes)
                        instances = instances.Include(include);
                }
                if(order != null) {
                    instances = order.Invoke(instances);
                }
                return instances.ToList();

            });
        }
    }
}
