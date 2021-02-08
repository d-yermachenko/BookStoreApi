using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    /// <summary>
    /// Not recommended to use this class in pruduction
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal abstract class CustomInMemoryValueGenerator<TEntity, TKey>
                : ValueGenerator<TKey> where TEntity : class
        where TKey: IComparable{
        private TKey _current;

        private readonly object locker = new object();

        private readonly Func<TEntity, TKey> _KeyTaker;
        private readonly Func<TKey, TKey> _ValueIncrementor;

        protected CustomInMemoryValueGenerator(Expression<Func<TEntity, TKey>> keyTaker, Expression<Func<TKey, TKey>> incrementFunc) {
            _KeyTaker = keyTaker.Compile();
            _ValueIncrementor = incrementFunc.Compile();
        }


        public override bool GeneratesTemporaryValues => false;
        public override TKey Next(EntityEntry entry) {
            if ((_current?.CompareTo(default(TKey))??0) == 0) {
                var context = entry.Context.Set<TEntity>().Local.AsEnumerable();
                _current = context.Max(
                    x => _KeyTaker.Invoke(x));
            }
            lock (locker) {
                _current = _ValueIncrementor.Invoke(_current);
            }
            return _current;
        }
    }

    internal class AuthorInMemoryValueGenerator : CustomInMemoryValueGenerator<Author, int> {
        public AuthorInMemoryValueGenerator() : base(x => x.Id, x =>x+1) {

        }

    }

    internal class BookInMemoryValueGenerator : CustomInMemoryValueGenerator<Book, int> {
        public BookInMemoryValueGenerator() : base(x => x.Id, x => x + 1) {

        }

    }
}
