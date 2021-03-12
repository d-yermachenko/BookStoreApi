using Blazored.LocalStorage;
using BookStoreUI.WASM.Contracts;
using BookStoreUI.WASM.Data;
using BookStoreUI.WASM.Providers;
using BookStoreUI.WASM.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.WASM {
    public class Program {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Configuration.AddInMemoryCollection(GetInMemoryConfiguration());
            builder.Configuration.Build();
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient {
                BaseAddress = new Uri(builder.Configuration.GetValue<string>
                (ConventionalUrls.BaseUrlConfigurationKey, builder.HostEnvironment.BaseAddress)),
            });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddTransient<IHttpRequestMessageProviderAsync, HttpRequestMessageProvider>();
            builder.Services.AddTransient<IAuthentificationRepository, AuthentificationRepository>();
            builder.Services.AddTransient<IAuthorsRepository, AuthorsRepository>();
            builder.Services.AddTransient<IBookRepository, BooksRepository>();
            builder.Services.AddScoped<JwtSecurityTokenHandler>();
            builder.Services.AddScoped<ApiAuthentificationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<ApiAuthentificationStateProvider>());
            builder.Services.AddLocalization(setupAction => {
                setupAction.ResourcesPath = "Resources";
            });
            builder.Services.AddMvcCore()
                .AddDataAnnotationsLocalization()
                ;
            builder.Services.AddAuthorizationCore(options => {
                options.AddPolicy("elementsCreation", policy => {
                    policy.RequireRole("Administrator");
                });
                options.AddPolicy("elementsEdition", policy => {
                    policy.RequireRole("Administrator");
                });
                options.AddPolicy("elementsDeleting", policy => {
                    policy.RequireRole("Administrator");
                });
            });

            await builder.Build().RunAsync();
        }


        public static IDictionary<string, string> GetInMemoryConfiguration() {
            Dictionary<string, string> config = new Dictionary<string, string> {
                { Data.ConventionalUrls.BaseUrlConfigurationKey, "https://dyobookstoreapi.azurewebsites.net/" }

            };
            return config;
        }

    }
}
