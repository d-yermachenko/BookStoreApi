using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using BookStoreApi.Contracts;
using BookStoreApi.Code.DataContoroller.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookStoreApi.Data.Authentification;
using BookStoreApi.Data;

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
            services.AddDbContext<BookStoreApi.Data.BookStoreIdentityDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddLocalization(options => {
                options.ResourcesPath = "Resources";
            });
            services.AddCors(o => {
                o.AddPolicy("ApiPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<AppRole>()
                .AddEntityFrameworkStores<BookStoreIdentityDbContext>()
                .AddDefaultTokenProviders();
            //services.AddLogging();
            services.AddSwaggerGen(setup => {
                setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() {
                    Title = "Book Store api",
                    Description = "Book store api",
                    Version = "1.0.0.0"
                });
                var xFile = System.IO.Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                setup.IncludeXmlComments(xFile, true);
            });
            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.MaxDepth = 1;
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o => { o.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = Configuration.GetValue<string>("Jwt:Issuer"),
                    ValidAudience = Configuration.GetValue<string>("Jwt:Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtKey")))
                }; 
                });

            services.AddAutoMapper(typeof(BookStoreApi.Code.AutoMapper.AutoMapperConfig));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddTransient<IAppDataSeeder, BookStoreApi.Code.AppDataSeeder>();
            services.AddScoped<IBookStoreUnitOfWorkAsync, EntityBookStoreUoWAsync>();
            services.AddScoped<IImageService, BookStoreApi.Code.FileSystemImageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAppDataSeeder dataSeeder) {
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
            app.UseCors("ApiPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(configure => {
                configure.MapControllers();
            });
            dataSeeder?.SeedData().Wait();

        }
    }
}
