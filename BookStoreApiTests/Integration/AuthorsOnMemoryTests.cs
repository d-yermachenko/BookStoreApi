using BookStoreApi.Data.DTOs;
using BookStoreApiTests.TestServers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Integration {
    [TestClass]
    public class AuthorsOnMemoryTests {

        [TestMethod]
        public async Task GetAuthorsFromInMemoryDb() {
            var client = new TestInMemoryDbServerClientFactory().TestClient;
            var response = await client.GetAsync("/api/Authors");
            bool succeed = response.IsSuccessStatusCode;
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            if (!succeed) 
                Assert.Fail(responseString);
            var authors = JsonConvert.DeserializeObject<IEnumerable<AuthorDTO>>(responseString);
            Assert.AreEqual(authors.Count(), 1);
        }
    }
}
