using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts {
    public interface IBookStoreUnitOfWorkAsync  {
        IRepositoryAsync<Data.Author, int> Authors { get; }

        IRepositoryAsync<Data.Book, int> Books { get; }

        public Task<bool> SaveData();
    }
}
