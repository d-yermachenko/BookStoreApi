using BookStoreApi.Data.Authentification;
using BookStoreApi.Data.DTOs;
using BookStoreApiTests.Mocks.MockIdentity;
using BookStoreApiTests.TestServers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Integration {
    [TestClass]
    public class UsersOnMemoryTests {


        #region Login
        [TestMethod]
        public async Task LoginUserNotFound404() {
            var client = new TestInMemoryDbServerClientFactory<Mocks.MockDataSeeder>().TestClient;
            var response = await client.PostAsync("/api/Users", new StringContent(
                JsonConvert.SerializeObject(new UserLoginDTO() {
                    Login = "barmalei", 
                    Password = "Trat@ta8" }
                ), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginBadRequest400() {
            var client = new TestInMemoryDbServerClientFactory<Mocks.MockDataSeeder>().TestClient;
            var response = await client.PostAsync("/api/Users/login", new StringContent(
                JsonConvert.SerializeObject(new UserLoginDTO() {
                    Login = "barmalei",
                }
                ), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginUnauthorized403() {
            var client = new TestInMemoryDbServerClientFactory<Mocks.MockDataSeeder>().TestClient;
            var response = await client.PostAsync("/api/Users/login", new StringContent(
                JsonConvert.SerializeObject(new UserLoginDTO() {
                    Login = "admin",
                    Password = "P@ssword111"
                }
                ), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginAccepted202() {
            var client = new TestInMemoryDbServerClientFactory<Mocks.MockDataSeeder>().TestClient;
            var userData = new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword128!"
            };
            string serializeContent = JsonConvert.SerializeObject(userData);
            var response = await client.PostAsync("/api/Users/login", new StringContent(serializeContent, Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginInternalServerError500() {
            var userController = new BookStoreApi.Controllers.UsersController(
                new FakeSignInManager<AppUser>(), 
                new FakeUserManager<AppUser>(),
                ConfigurationProvider.BuildConfiguration(),
                new Mocks.MockLogger<BookStoreApi.Controllers.UsersController>()
                
                );
            var result = (ObjectResult) await userController.Login(new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword111"
            });
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion



    }
}
