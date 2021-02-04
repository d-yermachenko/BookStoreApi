using BookStoreApi.Code.DataContoroller.Entity;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    public class MockBookStoreInMemoryUnitOfWork : IBookStoreUnitOfWorkAsync {

        private readonly BookStoreContext _BookStoreContext;
        private readonly IRepositoryAsync<Author> _AuthorsRepository;
        private readonly IRepositoryAsync<Book> _BooksRepository;

        public MockBookStoreInMemoryUnitOfWork(MockBookStoreInMemoryContext storeContext) {
            _BookStoreContext = storeContext;
            _AuthorsRepository = new MockRepositoryInMemoryAsync<Author>(_BookStoreContext.Authors, x => x.Id);
            _BooksRepository = new MockRepositoryInMemoryAsync<Book>(_BookStoreContext.Books, x => x.Id);
        }

        public IRepositoryAsync<Author> Authors {
            get => _AuthorsRepository;
        }
        
        public IRepositoryAsync<Book> Books {
            get => _BooksRepository;
        }


        public async Task<bool> SaveData() {
            return (await _BookStoreContext.SaveChangesAsync() > 0);
        }

    }
}
