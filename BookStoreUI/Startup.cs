using BookStoreUI.Contracts;
using BookStoreUI.Data;
using BookStoreUI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BookStoreUI.Providers;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookStoreUI {
    public class Startup {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddBlazoredLocalStorage();
            services.AddTransient<IHttpRequestMessageProviderAsync, HttpRequestMessageProvider>();
            services.AddTransient<IAuthentificationRepository, AuthentificationRepository>();
            services.AddTransient<IAuthorsRepository, AuthorsRepository>(); 
            services.AddTransient<IBookRepository, BooksRepository>(); 
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<ApiAuthentificationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<ApiAuthentificationStateProvider>());
            services.AddLocalization(setupAction => {
                setupAction.ResourcesPath = "Resources";
            });
            services.AddMvc()
                .AddDataAnnotationsLocalization()
                ;
            services.AddAuthorization(options=> {
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
