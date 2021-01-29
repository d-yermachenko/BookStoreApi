using BookStoreApi.Contracts;
using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code.DataContoroller.Entity {
    public class EntityBookStoreUoWAsync : IBookStoreUnitOfWorkAsync, IDisposable {
        private readonly IRepositoryAsync<Author> _Authors;
        private readonly IRepositoryAsync<Book> _Books;
        private readonly BookStoreContext _BookStoreContext;



        public EntityBookStoreUoWAsync(BookStoreContext dbContext) {
            _BookStoreContext = dbContext;
            _Authors = new EntityRepositoryAsync<Author>(dbContext.Authors, a => a.Id);
            _Books = new EntityRepositoryAsync<Book>(dbContext.Books, b => b.Id);
        }

        public IRepositoryAsync<Author> Authors {
            get => _Authors;
        }

        public IRepositoryAsync<Book> Books {
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
