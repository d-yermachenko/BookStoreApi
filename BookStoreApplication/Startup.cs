using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.Code.DataContoroller.Entity;
using Newtonsoft.Json;

namespace BookStoreApplication {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<BookStoreApi.Data.BookStoreContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<BookStoreApi.Data.BookStoreContext>();
            services.AddLogging();
            services.AddSwaggerGen(setup => {
                setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() {
                    Title = "Book Store api",
                    Description = "Book store api",
                    Version = "1.0.0.0"
                });
                var xFile = System.IO.Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                setup.IncludeXmlComments(xFile, true);
            });
            services.AddControllers();
            services.AddAutoMapper(typeof(BookStoreApi.Code.AutoMapper.AutoMapperConfig));
            //services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<IBookStoreUnitOfWorkAsync, EntityBookStoreUoWAsync>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();

            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(setupAction => {
                setupAction.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store api");
                setupAction.RoutePrefix = "";
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(configure => {
                configure.MapControllers();
            });


        }
    }
}
