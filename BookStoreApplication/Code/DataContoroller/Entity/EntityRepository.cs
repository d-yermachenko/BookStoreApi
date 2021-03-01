using BookStoreApi.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStoreApi.Code.DataContoroller.Entity {
    public class EntityRepositoryAsync<TEntity, TKey> : IRepositoryAsync<TEntity, TKey> 
        where TEntity : class
        where TKey: IComparable<TKey>{

        private readonly Func<TEntity, TKey> _KeyGetter;

        public EntityRepositoryAsync(DbSet<TEntity> entities, Func<TEntity, TKey> keyGetter) {
            _ObjectSet = entities;
            _KeyGetter = keyGetter;
        }

        private readonly DbSet<TEntity> _ObjectSet;

        public async Task<bool> CreateAsync(TEntity entity) {
            bool result;
            try {
                await _ObjectSet.AddAsync(entity);
                result = true;
            }
            catch {
                throw;
            }
            finally {

            }
            return result;
        }

        public async Task<bool> DeleteAsync(TKey id) {
            bool result;
            try {

                TEntity entity = await _ObjectSet.FindAsync(id);
                if (entity == null)
                    throw new KeyNotFoundException($"Entity of type {typeof(TEntity).FullName} with key {id} was not found");
                _ObjectSet.Remove(entity);
                result = await Task.FromResult(true);
            }
            catch (Exception) {
                throw;
            }
            return result;
        }

        public async Task<TEntity> FindAsync(TKey id, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            IQueryable<TEntity> query = _ObjectSet.AsQueryable();
            if (includes != null) {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(x=>_KeyGetter.Invoke(x).CompareTo(id) == 0);
        }

        public async Task<bool> UpdateAsync(TEntity entity) {
            bool result ;
            try {
                _ObjectSet.Update(entity);
                result = await Task.FromResult(true);
            }
            catch (Exception) {
                throw;
            }

            finally {

            }
            return result;
        }

        public async Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            IQueryable<TEntity> query = _ObjectSet.AsQueryable();
            if (filter != null) {
                query = query.Where(filter);
            }
            if (includes != null) {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            if (order != null)
                query = order.Invoke(query);

            return await query.ToListAsync();
        }
    }
}
