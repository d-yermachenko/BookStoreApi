using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUITests.Integration.RepositoryTests {
    [TestClass]
    public class TestAuthorsServiceRepository {

        public readonly IHttpClientFactory _HttpClientFactory;
        public readonly BookStoreUI.Services.ServiceRepositoryBase<BookStoreApi.Data.Author, int> _AuthorsRepository;

        public TestAuthorsServiceRepository() {
            _HttpClientFactory = new Fakes.FakeHttpClientFactory();
            var mockttpRequestMessageProvide = new Moq.Mock<BookStoreUI.Contracts.IHttpRequestMessageProviderAsync>();
            mockttpRequestMessageProvide.Setup(x => x.CreateMessageAsync(It.IsAny<HttpMethod>(), It.IsAny<string>()))
                .Returns<HttpMethod, string>((method, url)=> {
                    return Task.Run(() => new HttpRequestMessage(method, url));
                });
            _AuthorsRepository = new BookStoreUI.Services.ServiceRepositoryBase<BookStoreApi.Data.Author, int>(_HttpClientFactory, x => x.Id,
               mockttpRequestMessageProvide.Object);
        }

        [TestMethod]
        public async Task TestSimpleGetMethod() {
            var answer = await _AuthorsRepository.Get("api/authors");
            Assert.AreEqual(3, answer.Count());

        }
    }
}
