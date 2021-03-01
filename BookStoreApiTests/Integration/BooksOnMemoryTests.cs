using BookStoreApi.Data.DTOs;
using BookStoreApiTests.TestServers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Integration {
    [TestClass]
    public class BooksOnMemoryTests {

        #region Example DTOs
        public static BookDTO GetCreateExampleDTO() {
            return new BookDTO() {
                Id = 21,
                Title = "Idiot",
                Isbn = "978-5-17-087885-7",
                Year= 2015,
                Price = 6.05M,
                Summary = "One very clean man after long cure of psychological desease arrives in environement, which profits his kindness. He grt this desease back"
            };
        }

        #endregion

        [TestMethod]
        public async Task GetAuthorsFromInMemoryDb() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("/api/Books");
            bool succeed = response.IsSuccessStatusCode;
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            if (!succeed) 
                Assert.Fail(responseString);
            var authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDTO>>(responseString);
            Assert.AreEqual(3, authors.Count());
        }


        #region Get
        [TestMethod]
        public async Task GetOne200Ok() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("api/Books/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<BookDTO>(responseString);
            Assert.AreEqual(authors.Title, "Monday starts saturday");
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task Create201Created() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync((client)=>AuthorizeMethods.AutorizeAsync(()=>Mocks.MockDataSeeder.AdminLogin, client));
            var postResponse = await client.PostAsync("api/Books", content);
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create400BadRequest() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            exampleDTO.Title = default;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync(async (client) => await AuthorizeMethods.AutorizeAsync(() => new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword128!"
            }, client));
            var postResponse = await client.PostAsync("api/Books", content);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task CreateAndTakeBackAuthor() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync(async (client) =>
                await AuthorizeMethods.AutorizeAsync(() => Mocks.MockDataSeeder.AdminLogin, client));
            var postResponse = await client.PostAsync("api/Books", content);
            bool assertion = postResponse.IsSuccessStatusCode;
            Assert.IsTrue(assertion, "Save failed");
            var createdBook = JsonConvert.DeserializeObject<BookDTO>(await postResponse.Content.ReadAsStringAsync());
            var authorFromDbResponce = await client.GetAsync($"api/Books/{createdBook.Id}");
            assertion &= authorFromDbResponce.IsSuccessStatusCode;
            Assert.IsTrue(assertion, "Failed to get author back");
            var authorFromDb = JsonConvert.DeserializeObject<BookDTO>(await authorFromDbResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(exampleDTO.Title, authorFromDb.Title);
        }

        #endregion
    }
}
