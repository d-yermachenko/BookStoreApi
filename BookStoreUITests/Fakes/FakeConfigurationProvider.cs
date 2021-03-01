using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUITests.Fakes {
    class FakeConfigurationProvider {

        public static IConfiguration BuildConfiguration() {
            Dictionary<string, string> config = new Dictionary<string, string> {
                { "ConnectionString:DefaultUrl", "" }
            };
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddInMemoryCollection(config);
            return builder.Build();
        }
    }
}
