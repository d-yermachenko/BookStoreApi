using BookStoreApi.Contracts;
using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code.DataContoroller.Entity {
    public class EntityBookStoreUoWAsync : IBookStoreUnitOfWorkAsync, IDisposable {
        private readonly IRepositoryAsync<Author, int> _Authors;
        private readonly IRepositoryAsync<Book, int> _Books;
        private readonly BookStoreContext _BookStoreContext;



        public EntityBookStoreUoWAsync(BookStoreContext dbContext) {
            _BookStoreContext = dbContext;
            _Authors = new EntityRepositoryAsync<Author, int>(dbContext.Authors);
            _Books = new EntityRepositoryAsync<Book, int>(dbContext.Books);
        }

        public IRepositoryAsync<Author, int> Authors {
            get => _Authors;
        }

        public IRepositoryAsync<Book, int> Books {
            get => _Books;
        }


        public async Task<bool> SaveData() {
            return await _BookStoreContext.SaveChangesAsync() > 0;
        }


        #region Disposing

        private bool _Disposed;
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if (_Disposed)
                return;
            if (disposing)
                _BookStoreContext.Dispose();
            _Disposed = true;
        }



        #endregion
    }
}
