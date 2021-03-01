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
    public class AuthorsOnMemoryTests {

        #region Example DTOs
        public static AuthorDTO GetCreateExampleDTO() {
            return new AuthorDTO() {
                Id = 21,
                Firstname = "Ray",
                Lastname = "Bradbury",
                Bio = @"Ray Douglas Bradbury (/ˈbrædˌbɛri/; August 22, 1920 – June 5, 2012) was an American author and screenwriter. One of the most celebrated 20th- and 21st-century American writers, he worked in a variety of genres including fantasy, science fiction, horror, and mystery fiction."
            };
        }

        public static AuthorDTO GetUpdateExampleDTO() {
            return new AuthorDTO() {
                Id = 3,
                Firstname = "Charles",
                Lastname = "Beaumont",
                Bio = @"(January 2, 1929 – February 21, 1967) was an American author of speculative fiction, including short stories in the horror and science fiction subgenres."
            };
        }
        #endregion

        [TestMethod]
        public async Task GetAuthorsFromInMemoryDb() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("/api/Authors");
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
            var response = await client.GetAsync("api/Authors/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<AuthorDTO>(responseString);
            Assert.AreEqual(authors.Lastname, "Strugatski");
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task Create201Created() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync((client)=>AuthorizeMethods.AutorizeAsync(()=>new UserLoginDTO() { 
                Login = "admin",
                Password = "P@ssword128!"
            }, client));
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create400BadRequest() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            exampleDTO.Lastname = default;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync(async (client) => await AuthorizeMethods.AutorizeAsync(() => new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword128!"
            }, client));
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task CreateAndTakeBackAuthor() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync(async (client) => await AuthorizeMethods.AutorizeAsync(() => new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword128!"
            }, client));
            var postResponse = await client.PostAsync("api/Authors", content);
            bool assertion = postResponse.IsSuccessStatusCode;
            Assert.IsTrue(assertion, "Save failed");
            var createdAuthor = JsonConvert.DeserializeObject<AuthorUpsertDTO>(await postResponse.Content.ReadAsStringAsync());
            var authorFromDbResponce = await client.GetAsync($"api/Authors/{createdAuthor.Id}");
            assertion &= authorFromDbResponce.IsSuccessStatusCode;
            Assert.IsTrue(assertion, "Failed to get author back");
            var authorFromDb = JsonConvert.DeserializeObject<AuthorUpsertDTO>(await authorFromDbResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(exampleDTO.Lastname, authorFromDb.Lastname);
        }

        #endregion
    }
}
