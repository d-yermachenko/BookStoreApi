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
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Integration {
    [TestClass]
    public class UsersOnMemoryTests {


        #region Login
        [TestMethod]
        public async Task LoginUserNotFound404() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(
                JsonConvert.SerializeObject(new UserLoginDTO() {
                    Login = "barmalei",
                    Password = "Trat@ta8"
                }
                ), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.PostAsync("/api/Users", content);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginBadRequest400() {
            var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(
                JsonConvert.SerializeObject(new UserLoginDTO() {
                    Login = "barmalei",
                }
                ), Encoding.UTF8, MediaTypeNames.Application.Json);

            var client = await clientFactory.GetTestClientAsync();
            var response = await client.PostAsync("/api/Users/login", content);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginUnauthorized403() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var content = new StringContent(
                JsonConvert.SerializeObject(new UserLoginDTO() {
                    Login = "admin",
                    Password = "P@ssword111"
                }
                ), Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.PostAsync("/api/Users/login", content);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginAccepted202() {
            using var clientFactory = new TestInMemoryAuthentificatedDbServerClientFactory<Mocks.MockDataSeeder>();
            var userData = new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword128!"
            };
            string serializeContent = JsonConvert.SerializeObject(userData);
            var content = new StringContent(serializeContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var client = await clientFactory.GetTestClientAsync();
            var response = await client.PostAsync("/api/Users/login", content);
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
