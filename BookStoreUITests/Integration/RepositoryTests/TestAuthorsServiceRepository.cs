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
    public class TestAuthorsServiceRepository {
        

        #region Get all

        [TestMethod]
        public async Task TestAuthorsGetAllMethod200Ok() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Get();
                Assert.AreEqual(3, answer.Count());
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsGetAllMethod404NotFound() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<EmptyDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Get();
                Assert.AreEqual(0, answer.Count());
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsGetAllMethod500InternalServerError() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<BookStoreApi.Code.AppDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Get();
                Assert.AreEqual(0, answer.Count());
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }
        #endregion

        #region Get one

        [TestMethod]
        public async Task TestAuthorsGetMethod200Ok() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Get(2);
                Assert.AreEqual("Boris", answer.Firstname );
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsGetMethod404NotFound() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<EmptyDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Get(1);
                Assert.AreEqual(null, answer);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsGetMethod500() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<BookStoreApi.Code.AppDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Get(1);
                Assert.AreEqual(null, answer);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }
        #endregion

        #region Create 
        private static  BookStoreUI.Data.DTOs.Author NewAuthorDTO => new BookStoreUI.Data.DTOs.Author() {
            Id = 11,
            Firstname = "Fedor",
            Lastname = "Dostoevskij",
            Bio = "The word of pain and saddness"
        };


        [TestMethod]
        public async Task TestAuthorsCreate201Created() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Create(NewAuthorDTO);
                Assert.AreEqual(StatusCodes.Status201Created, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsCreate400BadRequest() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewAuthorDTO;
                invalidAuthor.Firstname = null;
                var answer = await authorsController.Create(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status400BadRequest, answer.StatusCode);
            }
            
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsCreate401Unauthorized() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Create(NewAuthorDTO);
                Assert.AreEqual(StatusCodes.Status401Unauthorized, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsCreate403Forbidden() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewAuthorDTO;
                var answer = await authorsController.Create(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsCreate500InternalServerError() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<BookStoreApi.Code.AppDataSeeder>(AppDataSeeder.AdminDto),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Create(NewAuthorDTO);
                Assert.AreEqual(StatusCodes.Status500InternalServerError, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        #endregion

        #region Update 
        public static BookStoreUI.Data.DTOs.Author UpdateAuthorDTO => new BookStoreUI.Data.DTOs.Author() {
            Id = 1,
            Firstname = "Maxim",
            Lastname = "Gorki",
            Bio = "???"
        };


        [TestMethod]
        public async Task TestAuthorsUpdate200Ok() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryAuthentificatedHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var authorToUpdate = await authorsController.Get(3);
                authorToUpdate.Firstname = "Maxim";
                foreach (var book in authorToUpdate.Books)
                    book.Authors = null;
                var answer = await authorsController.Update(authorToUpdate);
                Assert.AreEqual(StatusCodes.Status200OK, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsUpdate400BadRequest() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = UpdateAuthorDTO;
                invalidAuthor.Firstname = null;
                var answer = await authorsController.Update(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status400BadRequest, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsUpdate401Unauthorized() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Update(UpdateAuthorDTO);
                Assert.AreEqual(StatusCodes.Status401Unauthorized, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsUpdate403Forbidden() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewAuthorDTO;
                var answer = await authorsController.Update(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsUpdate404NotFound() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var invalidAuthor = NewAuthorDTO;
                invalidAuthor.Id = 111;
                var answer = await authorsController.Update(invalidAuthor);
                Assert.AreEqual(StatusCodes.Status404NotFound, answer.StatusCode);
            }

            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsUpdate500InternalServerError() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<AppDataSeeder>(AppDataSeeder.AdminDto),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Update(UpdateAuthorDTO);
                Assert.AreEqual(StatusCodes.Status500InternalServerError, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        #endregion


        #region Delete

        [TestMethod]
        public async Task TestAuthorsDelete200Ok() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.AdminLogin),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Delete(1);
                Assert.AreEqual(StatusCodes.Status204NoContent, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsDelete401Unauthorized() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Delete(1);
                Assert.AreEqual(StatusCodes.Status401Unauthorized, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsDelete403Forbidden() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Delete(1);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }



        [TestMethod]
        public async Task TestAuthorsDelete404NotFound() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryHttpClientFactory<MockDataSeeder>(MockDataSeeder.Customer1Login),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Delete(14);
                Assert.AreEqual(StatusCodes.Status403Forbidden, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }

        [TestMethod]
        public async Task TestAuthorsDelete500InternalServerError() {
            try {
                BookStoreUI.Services.AuthorsRepository authorsController = new BookStoreUI.Services.AuthorsRepository(
                    configuration: FakeConfigurationProvider.BuildConfiguration(),
                    clientFactory: new FakeInMemoryFaultyHttpClientFactory<AppDataSeeder>(AppDataSeeder.AdminDto),
                    messageProvider: new FakeHttpRequestMessageProviderAsync()
                    );
                var answer = await authorsController.Delete(1);
                Assert.AreEqual(StatusCodes.Status500InternalServerError, answer.StatusCode);
            }
            catch (AggregateException ae) {
                Assert.Fail(ae.Flatten().Message);
            }
        }
        #endregion
    }
}
