using BookStoreApi.Data.DTOs;
using BookStoreApiTests.TestServers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using System.Net.Http.Headers;
using BookStoreApi.Code;
using BookStoreApiTests.Mocks;

namespace BookStoreApiTests.Integration {
    [TestClass]
    public class AuthorsControllerTests {
        #region Example DTOs
        public static AuthorDTO GetCreateExampleDTO() {
            return new AuthorDTO() {
                Id = 21,
                Firstname = "Ray",
                Lastname = "Bradbury",
                Bio = @"Ray Douglas Bradbury (/ˈbrædˌbɛri/; August 22, 1920 – June 5, 2012) was an American author and screenwriter. One of the most celebrated 20th- and 21st-century American writers, he worked in a variety of genres including fantasy, science fiction, horror, and mystery fiction.",
                Books = new List<BookDTO>() {
                new BookDTO() { Id = 1, Title = "1-1", Isbn="978-3-16-148420-0"},
                new BookDTO() { Id = 2, Title = "2-1", Isbn="978-3-16-148421-0"}
            }

        };
        }

        public static AuthorDTO GetUpdateExampleDTO() {
            return new AuthorDTO() {
                Id = 2,
                Firstname = "Charles",
                Lastname = "Beaumont",
                Bio = @"(January 2, 1929 – February 21, 1967) was an American author of speculative fiction, including short stories in the horror and science fiction subgenres."
            };
        }
        #endregion

        #region List
        [TestMethod]
        public async Task GetAll404NotFound() {
            var provider = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>();
            var client = await provider.GetTestClientAsync(initAction: null);
            var response = await client.GetAsync("/api/Authors");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetAll200Ok() {
            //using var clientFactory = new TestFilledClientFactory();
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("/api/Authors");
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDTO>>(responseString);
            Assert.AreEqual(authors.Count(), 3);
        }

        [TestMethod]
        public async Task GetAll500InternalServerError() {
            using var clientFactory = new TestFaultyClientFactory<BookStoreApi.Code.AppDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("/api/Authors");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            System.Diagnostics.Trace.Write(responseString);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region Get
        [TestMethod]
        public async Task GetOne200Ok() {
            var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("api/Authors/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<AuthorDTO>(responseString);
            Assert.AreEqual(authors.Lastname, "Strugatski");
        }

        [TestMethod]
        public async Task GetOne404NotFound() {
            var client = await new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>().GetTestClientAsync();
            var response = await client.GetAsync("api/Authors/8");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetOne500InternalServerError() {
            var client = await new TestFaultyClientFactory<AppDataSeeder>().GetTestClientAsync();
            var response = await client.GetAsync("api/Authors/8");
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task Create201Created() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(
                () => MockDataSeeder.AdminLogin,
                "AuthorsCreate201Created", "AuthorsCreate201CreatedIdentity");
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create400BadRequest() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>(() => AppDataSeeder.AdminDto);
            var exampleDTO = GetCreateExampleDTO();
            exampleDTO.Lastname = default;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create401Unauthorized() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.Unauthorized, postResponse.StatusCode);
        }

        //[TestMethod]
        public async Task Create403Forbidden() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(() => MockDataSeeder.Customer1Login);
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.Forbidden, postResponse.StatusCode);
        }



        [TestMethod]
        public async Task Create422UnprocessableEntity() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(() => MockDataSeeder.AdminLogin);
            var exampleDTO = GetCreateExampleDTO();
            exampleDTO.Books = new List<BookDTO>() {
                new BookDTO() { Id = 1, Title = "1-1", Isbn="978-3-16-148420-0"},
                new BookDTO() { Id = 18, Title = "18-1", Isbn="978-3-16-148421-0"}
            };
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create500InternalServerError() {
            using var clientFactory = new TestFaultyClientFactory<BookStoreApi.Code.AppDataSeeder>();
            var exampleDTO = GetCreateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync(async (client) =>
                await AuthorizeMethods.AutorizeAsync(() => AppDataSeeder.AdminDto, client));
            var postResponse = await client.PostAsync("api/Authors", content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, postResponse.StatusCode);
        }

        #endregion

        #region Put
        [TestMethod]
        public async Task Update200Ok() {

            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(() => MockDataSeeder.AdminLogin);
            var exampleDTO = GetUpdateExampleDTO();
            var client = await clientFactory.GetTestClientAsync();
            var authorToUpdateRequest = await client.GetAsync($"api/Authors/{exampleDTO.Id}");
            var authorToUpdate = JsonConvert.DeserializeObject<AuthorDTO>(await authorToUpdateRequest.Content.ReadAsStringAsync());
            authorToUpdate.Bio = exampleDTO.Bio;
            var content = new StringContent(JsonConvert.SerializeObject(authorToUpdate), Encoding.UTF8, MediaTypeNames.Application.Json);
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update400BadRequest() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>(() => AppDataSeeder.AdminDto);
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Lastname = default;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update401Unauthorized() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>();
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Lastname = default;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(HttpStatusCode.Unauthorized, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update403Forbidden() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>(() => AppDataSeeder.CustomerDto);
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Lastname = default;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(HttpStatusCode.Forbidden, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update404NotFound() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>(() => AppDataSeeder.AdminDto);
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Id = 48;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update405MethodNotAllowed() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>();
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Id = 48;
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"api/Authors/{exampleDTO.Id}") {
                Content = content
            };
            var client = await clientFactory.GetTestClientAsync();
            var postResponse = await client.SendAsync(requestMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.MethodNotAllowed, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update422UnprocessableEntity() {

            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(() => MockDataSeeder.AdminLogin);
            var exampleDTO = GetUpdateExampleDTO();
            var client = await clientFactory.GetTestClientAsync();
            var authorToUpdateRequest = await client.GetAsync($"api/Authors/{exampleDTO.Id}");
            var authorToUpdate = JsonConvert.DeserializeObject<AuthorDTO>(await authorToUpdateRequest.Content.ReadAsStringAsync());
            authorToUpdate.Bio = exampleDTO.Bio;
            authorToUpdate.Books = new List<BookDTO>() {
                new BookDTO() { Id = 1, Title = "1-1", Isbn="978-3-16-148420-0"},
                new BookDTO() { Id = 18, Title = "18-1", Isbn="978-3-16-148421-0"}
            };
            var content = new StringContent(JsonConvert.SerializeObject(authorToUpdate), Encoding.UTF8, MediaTypeNames.Application.Json);
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, postResponse.StatusCode);
        }


        [TestMethod]
        public async Task Update500InternalServerError() {
            using var clientFactory = new TestFaultyClientFactory<BookStoreApi.Code.AppDataSeeder>();
            var exampleDTO = GetUpdateExampleDTO();
            var content = new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync((client) => AuthorizeMethods.AutorizeAsync(() => AppDataSeeder.AdminDto, client));
            var postResponse = await client.PutAsync($"api/Authors/{exampleDTO.Id}", content);
            Assert.AreEqual(HttpStatusCode.InternalServerError, postResponse.StatusCode);
        }

        #endregion

        #region Delete
        [TestMethod]
        public async Task Delete204NoContent() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(() => MockDataSeeder.AdminLogin);
            var exampleDTO = GetUpdateExampleDTO();
            var postResponse = await (await clientFactory.GetTestClientAsync()).DeleteAsync($"api/Authors/{exampleDTO.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Delete401Unauthorized() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>();
            var exampleDTO = GetUpdateExampleDTO();
            var postResponse = await (await clientFactory.GetTestClientAsync()).DeleteAsync($"api/Authors/46");
            Assert.AreEqual(HttpStatusCode.Unauthorized, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Delete403Forbidden() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<MockDataSeeder>(() => MockDataSeeder.Customer1Login);
            var exampleDTO = GetUpdateExampleDTO();
            var postResponse = await (await clientFactory.GetTestClientAsync()).DeleteAsync($"api/Authors/2");
            Assert.AreEqual(HttpStatusCode.Forbidden, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Delete404NotFound() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<AppDataSeeder>(() => AppDataSeeder.AdminDto);
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Id = 48;
            var postResponse = await (await clientFactory.GetTestClientAsync()).DeleteAsync($"api/Authors/46");
            Assert.AreEqual(HttpStatusCode.NotFound, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Delete500InternalServerError() {
            using var clientFactory = new TestFaultyClientFactory<BookStoreApi.Code.AppDataSeeder>();
            var exampleDTO = GetUpdateExampleDTO();
            var client = await clientFactory.GetTestClientAsync(async (client) => await AuthorizeMethods.AutorizeAsync(() => AppDataSeeder.AdminDto, client));
            var postResponse = await client.DeleteAsync($"api/Authors/{exampleDTO.Id}");
            Assert.AreEqual(HttpStatusCode.InternalServerError, postResponse.StatusCode);
        }

        #endregion


    }
}
