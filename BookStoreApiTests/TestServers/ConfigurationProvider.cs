using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;


namespace BookStoreApiTests.TestServers {
    public static class ConfigurationProvider {
        public static IConfiguration BuildConfiguration() {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        public static IConfiguration BuildBearerConfiguration() {
            Dictionary<string, string> config = new Dictionary<string, string> {
                { "JwtKey", "c8fc66ec-98d2-432b-b224-122d0a6957d6" },
                { "Jwt:Issuer", "Book store app" }
            };
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
