using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUITests.UnitTests {
    [TestClass]
    public class FlURLTests {
        [TestMethod]
        public void TestParaletreless() {
            string urlExample1 = Flurl.Url.Combine("http://localhost:44317", "api/books");
            Assert.AreEqual("http://localhost:44317/api/books", urlExample1);
        }

        [TestMethod]
        public void TestUrlToObject() {
            string urlExample1 = Flurl.Url.Combine("http://localhost:44317", "api/books", 18.ToString());
            Assert.AreEqual("http://localhost:44317/api/books/18", urlExample1);
        }

    }
}
