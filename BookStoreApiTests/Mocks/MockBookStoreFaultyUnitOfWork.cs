using BookStoreApi.Contracts;
using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockBookStoreFaultyUnitOfWork : IBookStoreUnitOfWorkAsync {

        private readonly IRepositoryAsync<Author> _Authors;
        private readonly IRepositoryAsync<Book> _Books;
        public MockBookStoreFaultyUnitOfWork() {
            _Authors = new MockRepositoryFaultyAsync<Author>();
            _Books = new MockRepositoryFaultyAsync<Book>();
        }

        public IRepositoryAsync<Author> Authors {
            get => _Authors;
        }

        public IRepositoryAsync<Book> Books {
            get => _Books;
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public Task<bool> SaveData() {
            throw new System.Data.DataException("Imagine that something happened while operating database");
        }
    }
}
