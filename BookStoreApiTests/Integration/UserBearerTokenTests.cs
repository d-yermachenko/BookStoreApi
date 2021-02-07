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
    public class UserBearerTokenTests {


        #region Login


        [TestMethod]
        public async Task LoginAccepted202() {
            var client = new TestInMemoryDbServerClientFactory<Mocks.MockDataSeeder>().TestClient;
            var userData = new UserLoginDTO() {
                Login = "admin",
                Password = "P@ssword128!"
            };
            string serializeContent = JsonConvert.SerializeObject(userData);
            var response = await client.PostAsync("/api/Users", new StringContent(serializeContent, Encoding.UTF8, "application/json"));
            UserLoginData userLoginData = JsonConvert.DeserializeObject<UserLoginData>(await response.Content.ReadAsStringAsync());
            System.Diagnostics.Trace.Write(userLoginData.Token);
            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
        }

       

        #endregion


    }
}
