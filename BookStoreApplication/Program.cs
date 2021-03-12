using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApplication {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

            .ConfigureAppConfiguration((env, conf) => {
                conf.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, true)
                .AddJsonFile($"appsettings.{env.HostingEnvironment.EnvironmentName}.json", optional: true, true)
                .AddCommandLine(args);
                if (env.HostingEnvironment.IsDevelopment())
                    conf.AddUserSecrets(env.HostingEnvironment.ApplicationName);
                else if(env.HostingEnvironment.IsProduction()) {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            azureServiceTokenProvider.KeyVaultTokenCallback));
                    string vaultUri = Environment.GetEnvironmentVariable("VaultUri");
                    conf.AddAzureKeyVault(
                        vaultUri,
                        keyVaultClient,
                        new DefaultKeyVaultSecretManager());
                }
                conf.Build();
            })
            .ConfigureLogging((loggingBuilder) => {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddEventSourceLogger();
                loggingBuilder.AddConsole();
                loggingBuilder.AddJsonConsole();
            })
              .ConfigureWebHostDefaults(webBuilder => {
                  webBuilder.UseStartup<Startup>();
              });
    }
}
