using BookStoreUI.WASM.Contracts;
using BookStoreUI.WASM.Data;
using BookStoreUI.WASM.Data.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http;


namespace BookStoreUI.WASM.Services {
    public class AuthorsRepository : RepositoryBase<Author, int>, IAuthorsRepository{

        public AuthorsRepository(IConfiguration configuration, HttpClient client,
            IHttpRequestMessageProviderAsync messageProvider) : base(configuration, client, messageProvider, x => x.Id ) {

        }

        protected override string ActionPath => ConventionalUrls.Authors;
    }
}
