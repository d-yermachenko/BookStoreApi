using BookStoreApi.Contracts;
using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockBookStoreUnitOfWork : IBookStoreUnitOfWorkAsync {

        private readonly IRepositoryAsync<Author> _Authors;

        private readonly IRepositoryAsync<Book> _Books;

        public MockBookStoreUnitOfWork() {
            _Authors = new MockRepositoryAsync<Author>(a => a.Id);
            _Books = new MockRepositoryAsync<Book>(b => b.Id);

        }

        public virtual IRepositoryAsync<Author> Authors {
            get => _Authors;
        }

        public virtual IRepositoryAsync<Book> Books {
            get => _Books;
        }

        public virtual void Dispose() {
            GC.SuppressFinalize(this);
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task<bool> SaveData() {
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            return true;
        }
    }
}
