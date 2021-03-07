using BookStoreUI.WASM.Contracts;
using BookStoreUI.WASM.Data;
using BookStoreUI.WASM.Data.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Services {
    public class BooksRepository : RepositoryBase<Book, int>, IBookRepository{

        public BooksRepository(IConfiguration configuration, 
            HttpClient client,
            IHttpRequestMessageProviderAsync messageProvider) : base(configuration, client, messageProvider, x => x.Id ) {

        }

        protected override string ActionPath => ConventionalUrls.Books;
    }
}
