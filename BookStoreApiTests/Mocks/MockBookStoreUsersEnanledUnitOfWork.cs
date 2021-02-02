using BookStoreApi.Code.DataContoroller.Entity;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockBookStoreUsersEnanledUnitOfWork : IBookStoreUnitOfWorkAsync {

        private readonly BookStoreContext _BookStoreContext;
        private readonly UserManager<IdentityUser> _UserProvider;
        private readonly RoleManager<IdentityRole> _RolesProvider;
        private readonly IRepositoryAsync<Author> _AuthorsRepository;
        private readonly IRepositoryAsync<Book> _BooksRepository;

        public MockBookStoreUsersEnanledUnitOfWork(BookStoreContext storeContext,
                                                  UserManager<IdentityUser> userProvider,
            RoleManager<IdentityRole> rolesProvider) {
            _BookStoreContext = storeContext;
            _UserProvider = userProvider;
            _RolesProvider = rolesProvider;
            _AuthorsRepository = new EntityRepositoryAsync<Author>(_BookStoreContext.Authors, x => x.Id);
            _BooksRepository = new EntityRepositoryAsync<Book>(_BookStoreContext.Books, x => x.Id);
            SeedData().Wait();
        }

        public IRepositoryAsync<Author> Authors {
            get => _AuthorsRepository;
        }


        public IRepositoryAsync<Book> Books {
            get=> _BooksRepository; 
        }

        bool _Disposed = false;
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if (_Disposed)
                return;
            if (disposing) {
                _BookStoreContext.Dispose();
                _UserProvider.Dispose();
                _RolesProvider.Dispose();
            }
            _Disposed = true;
        }

        public async Task<bool> SaveData() {
            return (await _BookStoreContext.SaveChangesAsync() > 0);
        }

        protected Task SeedData() {
            Authors.CreateAsync(new Author() {
                Id = 87,
                Firstname = "Korney",
                Lastname = "Chukovski"
            });
            return Task.CompletedTask;
        }

    }
}
