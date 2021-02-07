using BookStoreApi.Data.DTOs;
using BookStoreApiTests.TestServers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Integration {

    [TestClass]
    public class HomeControllerTests {
        private readonly HttpClient _client;
        public HomeControllerTests() {
            _client = new TestFilledClientFactory().TestClient;
        }

        [TestMethod]
        public async Task TestIndexMethod() {
            var response = await _client.GetAsync("/api/Home");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            var authors = JsonConvert.DeserializeObject<IEnumerable<string>>(responseString);
            Assert.AreEqual(authors.Count(), 2);

        }

        [TestMethod]
        public async Task TestLoginAndRestrictedMethod() {
            var client = new TestInMemoryDbServerClientFactory<Mocks.MockDataSeeder>().TestClient;
            var loginResponce = await client.PostAsync("api/Users", new StringContent(JsonConvert.SerializeObject(
                new UserLoginDTO() {
                    Login = "admin",
                    Password = "P@ssword128!"
                }), Encoding.UTF8, "application/json"));
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/Home");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var dataResponce = await client.SendAsync(requestMessage);
            dataResponce.EnsureSuccessStatusCode();
            var answer = JsonConvert.DeserializeObject<string[]>(await dataResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(2, answer.Length);

        }

    }
}
