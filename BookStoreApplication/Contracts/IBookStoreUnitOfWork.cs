using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts {
    public interface IBookStoreUnitOfWorkAsync : IDisposable {
        IRepositoryAsync<Data.Author> Authors { get; }

        IRepositoryAsync<Data.Book> Books { get; }

        public Task<bool> SaveData();
    }
}
