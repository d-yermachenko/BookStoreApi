using BookStoreUI.Contracts;
using BookStoreUI.Data;
using BookStoreUI.Data.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStoreUI.Services {
    public class AuthorsRepository : RepositoryBase<Author, int>, IAuthorsRepository{

        public AuthorsRepository(IConfiguration configuration, IHttpClientFactory clientFactory,
            IHttpRequestMessageProviderAsync messageProvider) : base(configuration, clientFactory, messageProvider, x => x.Id ) {

        }

        protected override string ActionPath => ConventionalUrls.Authors;
    }
}
