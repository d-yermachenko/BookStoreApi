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


namespace BookStoreApiTests.Integration {
    [TestClass]
    public class BooksControllerTests {
        #region Example DTOs
        public static BookDTO GetCreateExampleDTO() {
            return new BookDTO() {
                Id = 21,
                Title = "Fahrenheit 451",
                Isbn = "9780345342966",
                
                Year = 1998
            };
        }

        public static BookDTO GetUpdateExampleDTO() {
            return new BookDTO() {
                Id = 3,
                Title = "It Came from Outer Space",
                Isbn = "9780001842267",
                Summary = "Nobody expects the roof to crash in when they are at school. Even less so when it turns out to be a spaceship with an alien inside. But the schoolchildren soon get to like the alien who is friendly, if ugly. Just before the alien leaves the teacher takes a photograph to remember him by.",
                Year= 1992
            };
        }
        #endregion

        #region List
        [TestMethod]
        public async Task GetAll404NotFound() {
            var provider = new TestEmptyClientFactory();
            var client = provider.TestClient;
            var response = await client.GetAsync("/api/Books");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetAll200Ok() {
            var client = new TestFilledClientFactory().TestClient;
            var response = await client.GetAsync("/api/Books");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDTO>>(responseString);
            Assert.AreEqual(authors.Count(), 3);
        }

        [TestMethod]
        public async Task GetAll500InternalServerError() {
            var client = new TestFaultyClientFactory().TestClient;
            var response = await client.GetAsync("/api/Books");
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            System.Diagnostics.Trace.Write(responseString);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region Get
        [TestMethod]
        public async Task GetOne200Ok() {
            var client = new TestFilledClientFactory().TestClient;
            var response = await client.GetAsync("api/Authors/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<AuthorDTO>(responseString);
            Assert.AreEqual(authors.Lastname, "Strugatski");
        }

        [TestMethod]
        public async Task GetOne404NotFound() {
            var client = new TestEmptyClientFactory().TestClient;
            var response = await client.GetAsync("api/Authors/8");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetOne500InternalServerError() {
            var client = new TestFaultyClientFactory().TestClient;
            var response = await client.GetAsync("api/Authors/8");
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task Create201Created() {
            var client = new TestFilledClientFactory().TestClient;
            var exampleDTO = GetCreateExampleDTO(); ;
            var postResponse = await client.PostAsync("/api/Books", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create400BadRequest() {
            var client = new TestFilledClientFactory().TestClient;
            var exampleDTO = GetCreateExampleDTO();
            exampleDTO.Title = default;
            var postResponse = await client.PostAsync("/api/Books", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Create500InternalServerError() {
            var client = new TestFaultyClientFactory().TestClient;
            var exampleDTO = GetCreateExampleDTO();
            var postResponse = await client.PostAsync("/api/Books", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, postResponse.StatusCode);
        }

        #endregion

        #region Put
        [TestMethod]
        public async Task Update201Created() {
            var client = new TestFilledClientFactory().TestClient;
            var exampleDTO = GetUpdateExampleDTO(); ;
            var postResponse = await client.PutAsync($"/api/Books/{exampleDTO.Id}", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update400BadRequest() {
            var client = new TestFilledClientFactory().TestClient;
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Title = default;
            var postResponse = await client.PutAsync($"/api/Books/{exampleDTO.Id}", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update404NotFound() {
            var client = new TestFilledClientFactory().TestClient;
            var exampleDTO = GetUpdateExampleDTO();
            exampleDTO.Id = 48;
            var postResponse = await client.PutAsync($"/api/Books/{exampleDTO.Id}", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Update500InternalServerError() {
            var client = new TestFaultyClientFactory().TestClient;
            var exampleDTO = GetUpdateExampleDTO();
            var postResponse = await client.PutAsync($"/api/Books/{exampleDTO.Id}", new StringContent(JsonConvert.SerializeObject(exampleDTO), Encoding.UTF8, "application/json"));
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, postResponse.StatusCode);
        }

        #endregion

        #region Delete
        [TestMethod]
        public async Task Delete204NoContent() {
            var client = new TestFilledClientFactory().TestClient;
            var exampleDTO = GetUpdateExampleDTO(); ;
            var postResponse = await client.DeleteAsync($"/api/Books/{exampleDTO.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Delete404NotFound() {
            var client = new TestFilledClientFactory().TestClient;
            var postResponse = await client.DeleteAsync($"/api/Books/46");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task Delete500InternalServerError() {
            var client = new TestFaultyClientFactory().TestClient;
            var exampleDTO = GetUpdateExampleDTO();
            var postResponse = await client.DeleteAsync($"/api/Books/{exampleDTO.Id}");
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, postResponse.StatusCode);
        }

        #endregion


    }
}
