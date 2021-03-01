using BookStoreApi.Contracts;
using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockBookStoreFaultyUnitOfWork : IBookStoreUnitOfWorkAsync {

        private readonly IRepositoryAsync<Author, int> _Authors;
        private readonly IRepositoryAsync<Book, int> _Books;
        public MockBookStoreFaultyUnitOfWork() {
            _Authors = new MockRepositoryFaultyAsync<Author, int>();
            _Books = new MockRepositoryFaultyAsync<Book, int>();
        }

        public IRepositoryAsync<Author, int> Authors {
            get => _Authors;
        }

        public IRepositoryAsync<Book, int> Books {
            get => _Books;
        }


        public Task<bool> SaveData() {
            throw new System.Data.DataException("Imagine that something happened while operating database");
        }
    }
}
