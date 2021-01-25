using BookStoreApi.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStoreApi.Code.DataContoroller.Entity {
    public class EntityRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : class {

        public EntityRepositoryAsync(DbSet<TEntity> entities)
        {
            _ObjectSet = entities;
        }

        private readonly DbSet<TEntity> _ObjectSet;

        public async Task<bool> CreateAsync(TEntity entity)
        {
            bool result = false;
            try
            {
                await _ObjectSet.AddAsync(entity);
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {

            }
            return result;
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            bool result = false;
            try
            {
                _ObjectSet.Remove(entity);
                result = await Task.FromResult(true);
            }
            finally
            {

            }
            return result;
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter, IEnumerable<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> query = _ObjectSet;
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            bool result = false;
            try
            {
                _ObjectSet.Update(entity);
                result = await Task.FromResult(true);
            }
            finally
            {

            }
            return result;
        }

        public async Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order = null, IEnumerable<Expression<Func<TEntity, object>>> includes = null)
        {

            IQueryable<TEntity> query = _ObjectSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            if (order != null)
                query = order.Invoke(query);

            return await query.ToListAsync();
        }
    }
}
