using Microsoft.Extensions.Configuration;
using System.IO;


namespace BookStoreApiTests.TestServers {
    public static class ConfigurationProvider {
        public static IConfiguration BuildConfiguration() {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }
}
