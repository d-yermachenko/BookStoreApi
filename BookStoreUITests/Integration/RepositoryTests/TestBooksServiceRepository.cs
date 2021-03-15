using BookStoreApi.Code;
using BookStoreApi.Data;
using BookStoreApiTests.Mocks;
using BookStoreApiTests.TestServers;
using BookStoreUITests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookStoreUITests.Integration.RepositoryTests {
    [TestClass]
    public class TestBooksServiceRepository {
        

        #region Get all

        [TestMethod]
        public async Task TestBooksGetAllMethod200Ok() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Get();
                Assert.AreEqual(3, answer.Count());
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksGetAllMethod404NotFound() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<EmptyDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Get();
                Assert.AreEqual(0, answer.Count());
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksGetAllMethod500InternalServerError() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<BookStoreApi.Code.AppDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Get();
                Assert.AreEqual(0, answer.Count());
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }
        #endregion

        #region Get one

        [TestMethod]
        public async Task TestBooksGetMethod200Ok() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Get(2);
                Assert.AreEqual("Hard to be the God", answer.Title );
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksGetMethod404NotFound() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<EmptyDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Get(1);
                Assert.AreEqual(null, answer);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksGetMethod500() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<BookStoreApi.Code.AppDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Get(1);
                Assert.AreEqual(null, answer);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }
        #endregion

        #region Create 
        public static BookStoreUI.Data.DTOs.Book 
            NewBookDTO => new  BookStoreUI.Data.DTOs.Book() {
            Id = 21,
            Title = "Idiot",
            Isbn = "978-5-17-087885-7",
            Year = 2015,
            Price = 6.05M,
            Summary = "One very clean man after long cure of psychological desease arrives in environement, which profits his kindness. He grt this desease back"
        };


        [TestMethod]
        public async Task TestBooksCreate201Created() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Create(NewBookDTO);
                Assert.AreEqual(StatusCodes.Status201Created, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksCreate400BadRequest() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewBookDTO;
                invalidAuthor.Title = null;
                var answer = await booksRepository.Create(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status400BadRequest, answer.StatusCode);
            }
            
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksCreate401Unauthorized() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Create(NewBookDTO);
                Assert.AreEqual(StatusCodes.Status401Unauthorized, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        //[TestMethod]
        public async Task TestBooksCreate403Forbidden() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewBookDTO;
                var answer = await booksRepository.Create(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksCreate500InternalServerError() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<BookStoreApi.Code.AppDataSeeder>(AppDataSeeder.AdminDto),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Create(NewBookDTO);
                Assert.AreEqual(StatusCodes.Status500InternalServerError, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        #endregion

        #region Update 


        [TestMethod]
        public async Task TestBooksUpdate200Ok() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryAuthentificatedHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var authorToUpdate = await booksRepository.Get(3);
                authorToUpdate.Title = "Maxim";
                var answer = await booksRepository.Update(authorToUpdate);
                Assert.AreEqual(StatusCodes.Status200OK, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksUpdate400BadRequest() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryAuthentificatedHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var authorToUpdate = await booksRepository.Get(3);
                authorToUpdate.Title = String.Empty;
                var answer = await booksRepository.Update(authorToUpdate);
                Assert.AreEqual(StatusCodes.Status400BadRequest, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksUpdate401Unauthorized() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var authorToUpdate = await booksRepository.Get(3);
                authorToUpdate.Title = "Maxim";
                var answer = await booksRepository.Update(authorToUpdate);
                Assert.AreEqual(StatusCodes.Status401Unauthorized, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksUpdate403Forbidden() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewBookDTO;
                invalidAuthor.Id = 3;
                var answer = await booksRepository.Update(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksUpdate404NotFound() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewBookDTO;
                invalidAuthor.Id = 111;
                var answer = await booksRepository.Update(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status404NotFound, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksUpdate500InternalServerError() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<AppDataSeeder>(AppDataSeeder.AdminDto),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                BookStoreUI.Data.DTOs.Book bookToUpdate = new BookStoreUI.Data.DTOs.Book() {
                    Id = 3,
                    Title = "Some book",
                    Isbn = "978-5-17-087885-7"
                };
                var answer = await booksRepository.Update(bookToUpdate);
                Assert.AreEqual(StatusCodes.Status500InternalServerError, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        #endregion


        #region Delete

        [TestMethod]
        public async Task TestBooksDelete200Ok() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Delete(1);
                Assert.AreEqual(StatusCodes.Status204NoContent, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksDelete401Unauthorized() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Delete(1);
                Assert.AreEqual(StatusCodes.Status401Unauthorized, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksDelete403Forbidden() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Delete(1);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }



        [TestMethod]
        public async Task TestBooksDelete404NotFound() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Delete(14);
                Assert.AreEqual(StatusCodes.Status404NotFound, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestBooksDelete500InternalServerError() {
            try {
                BookStoreUI.Contracts.IBookRepository booksRepository = new BookStoreUI.Services.BooksRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<AppDataSeeder>(AppDataSeeder.AdminDto),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await booksRepository.Delete(1);
                Assert.AreEqual(StatusCodes.Status500InternalServerError, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }
        #endregion
    }
}
