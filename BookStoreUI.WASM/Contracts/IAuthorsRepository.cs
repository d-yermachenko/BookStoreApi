using BookStoreUI.WASM.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Contracts {
    public interface IAuthorsRepository : IRepository<Author,  int> {
    }
}
