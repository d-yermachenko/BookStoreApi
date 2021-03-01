using BookStoreApi.Data.DTOs;
using BookStoreApiTests.TestServers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Integration {
    /// <summary>
    /// Testing bearer token
    /// Allowed methods: Get - [Authorize]
    /// Get(id) - [AllowAnonimous]
    /// Post - [Autorize(Roles = "Administrator, Customer")]
    /// Put - [Authorize(Roles = "Customer")]
    /// Delete - [Authorize(Roles="Administrator")]
    /// </summary>
    [TestClass]
    public class HomeControllerTests {


        protected static UserLoginDTO AdminLoginData => new UserLoginDTO() {
            Login = "admin",
            Password = "P@ssword128!"
        };

        protected static UserLoginDTO CustomerLoginData => new UserLoginDTO() {
            Login = "customer1",
            Password = "P@ssword2"
        };



        #region Login


        [TestMethod]
        public async Task LoginAccepted202() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            string serializeContent = JsonConvert.SerializeObject(AdminLoginData);
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.PostAsync("/api/Users/login", new StringContent(serializeContent, Encoding.UTF8, MediaTypeNames.Application.Json));
            UserLoginData userLoginData = JsonConvert.DeserializeObject<UserLoginData>(await response.Content.ReadAsStringAsync());
            System.Diagnostics.Trace.Write(userLoginData.Token);
            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
        }



        #endregion

        #region Index (simple Get). For Index session, Authorisation set to [Authorize]
        [TestMethod]
        public async Task TestAnonimousForbiddenUnauthorized() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/Home");
            var client = await clientFactory.GetTestClientAsync();
            var dataResponce = await client.SendAsync(requestMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, dataResponce.StatusCode);
        }

        [TestMethod]
        public async Task TestAnonimousForbiddenOk() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(JsonConvert.SerializeObject(
                AdminLoginData), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var loginResponce = await client.PostAsync("/api/Users/login", content);
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/Home");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var dataResponce = await client.SendAsync(requestMessage);
            dataResponce.EnsureSuccessStatusCode();
            var answer = JsonConvert.DeserializeObject<string[]>(await dataResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(2, answer.Length);
        }



        #endregion

        #region Get(id). For Get/id verb, Authorizartion set to [AllowAnonymous]
        /// <summary>
        /// Test of scenario of executing AllowAnonymous method 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestAnonymousAllowedAnonymous() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.GetAsync("/api/Home/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("value", responseString);

        }

        [TestMethod]
        public async Task TestAnonymousAllowedLoggedIn() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var loginContent = new StringContent(JsonConvert.SerializeObject(
                AdminLoginData), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var loginResponce = await client.PostAsync("/api/Users/login", loginContent);
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/Home/1");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("value", responseString);
        }
        #endregion

        #region Put. For Put verb, Authorization set to [Authorize(Roles = "Customer")]. So, Admin will receive ForbiddenStatus

        [TestMethod]
        public async Task TestHttpPutUnauthorized() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/Home/2") {
                Content = new StringContent("value", Encoding.UTF8, "text/plain")
            };
            var client = await clientFactory.GetTestClientAsync();
            var dataResponce = await client.SendAsync(requestMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, dataResponce.StatusCode);
        }

        [TestMethod]
        public async Task TestHttpPutForbidden() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(JsonConvert.SerializeObject(
                AdminLoginData), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var loginResponce = await client.PostAsync("/api/Users/login", content); //<- Key moment
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/Home/2") {
                Content = new StringContent("value", Encoding.UTF8, "text/plain")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var dataResponce = await client.SendAsync(requestMessage);
            //dataResponce.EnsureSuccessStatusCode();
            //var answer = JsonConvert.DeserializeObject<string[]>(await dataResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, dataResponce.StatusCode);
        }

        [TestMethod]
        public async Task TestHttpPutAllowed() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(JsonConvert.SerializeObject(
                CustomerLoginData), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var loginResponce = await client.PostAsync("/api/Users/login", content);
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/Home/2") {
                Content = new StringContent(
                JsonConvert.SerializeObject("value"),    
                 Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var dataResponce = await client.SendAsync(requestMessage);
            //dataResponce.EnsureSuccessStatusCode();
            //var answer = JsonConvert.DeserializeObject<string[]>(await dataResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(System.Net.HttpStatusCode.OK, dataResponce.StatusCode);
        }
        #endregion

        #region Delete. For Delete verb, Authorization set to [Authorize(Roles = "Administrator")]. So, Customer will receive ForbiddenStatus

        [TestMethod]
        public async Task TestHttpDeleteUnauthorized() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, "api/Home/2") {
                Content = new StringContent("value", Encoding.UTF8, "text/plain")
            };
            var client = await clientFactory.GetTestClientAsync();
            var dataResponce = await client.SendAsync(requestMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, dataResponce.StatusCode);
        }

        [TestMethod]
        public async Task TestHttpDeleteForbidden() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(JsonConvert.SerializeObject(
                CustomerLoginData), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var loginResponce = await client.PostAsync("/api/Users/login", content); //<- Key moment
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, "api/Home/2") {
                Content = new StringContent("value", Encoding.UTF8, "text/plain")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var dataResponce = await client.SendAsync(requestMessage);
            //dataResponce.EnsureSuccessStatusCode();
            //var answer = JsonConvert.DeserializeObject<string[]>(await dataResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, dataResponce.StatusCode);
        }

        [TestMethod]
        public async Task TestHttpDeleteAllowed() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(JsonConvert.SerializeObject(
                AdminLoginData), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var loginResponce = await client.PostAsync("/api/Users/login", content);
            loginResponce.EnsureSuccessStatusCode();
            UserLoginData loginData = JsonConvert.DeserializeObject<UserLoginData>(await loginResponce.Content.ReadAsStringAsync());
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, "api/Home/2") {
                Content = new StringContent(
                JsonConvert.SerializeObject("value"),
                 Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Token);
            var dataResponce = await client.SendAsync(requestMessage);
            //dataResponce.EnsureSuccessStatusCode();
            //var answer = JsonConvert.DeserializeObject<string[]>(await dataResponce.Content.ReadAsStringAsync());
            Assert.AreEqual(System.Net.HttpStatusCode.OK, dataResponce.StatusCode);
        }
        #endregion
    }
}
