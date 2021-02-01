using BookStoreApi.Data.DTOs;
using BookStoreApiTests.TestServers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    }
}
