﻿using BookStoreApi.Code.DataContoroller.Entity;
using BookStoreApi.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using BookStoreApiTests.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using BookStoreApi.Data;
using BookStoreApiTests.Mocks.InMemory;
using BookStoreApi.Data.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace BookStoreApiTests.TestServers {
    public class TestInMemoryAuthentificatedDbServerClientFactory<TDataSeeder> : TestserverClientFactory, ITestClientFactoryAsync, IDisposable
        where TDataSeeder : class, IAppDataSeeder {

        private readonly Func<UserLoginDTO> _DtoProvider;


        public TestInMemoryAuthentificatedDbServerClientFactory(Func<UserLoginDTO> dtoProvider = null) : base() {
            _DtoProvider = dtoProvider;
        }

        protected override Task<HttpClient> InitializeTestInfrastruture() {
            return CreateServerAsync().ContinueWith<HttpClient>((serverCreationTask) => {
                try {
                    if (serverCreationTask.Status == TaskStatus.RanToCompletion) {
                        TestServer server = serverCreationTask.Result;
                        var client = server.CreateClient();
                        var login = _DtoProvider?.Invoke();
                        if (login != null) {
                            string serializeContent = JsonConvert.SerializeObject(login);
                            var response = client.PostAsync("/api/Users/login", new StringContent(serializeContent, Encoding.UTF8, MediaTypeNames.Application.Json)).Result;
                            if (!response.IsSuccessStatusCode)
                                throw new HttpRequestException(message: "Client failed to login", inner: null, statusCode: response.StatusCode);
                            UserLoginData userLoginData = JsonConvert.DeserializeObject<UserLoginData>(response.Content.ReadAsStringAsync().Result);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userLoginData.Token);
                        }
                        return client;
                    }
                    else
                        throw new OperationCanceledException("Failed to create test server");
                }
                catch (AggregateException e) {
                    throw e.Flatten();
                }
            });
        }

        protected override TestServer CreateServer() {
            return new TestServer(new WebHostBuilder()
                                    .UseStartup<BookStoreApplication.Startup>()
                                    .UseConfiguration(ConfigurationProvider.BuildBearerConfiguration())
                                    .ConfigureTestServices((services) => {
                                        //https://github.com/aspnet/Hosting/issues/1012

                                        services.AddSingleton(provider => {
                                            var bookStoreDbOptions = new DbContextOptionsBuilder<BookStoreContext>()
                                           .UseInMemoryDatabase("IMBookStore")
                                           .Options;
                                            var bookStoreDbContext = new MockBookStoreInMemoryContext(bookStoreDbOptions);
                                            bookStoreDbContext.Database.EnsureDeleted();
                                            return bookStoreDbContext;
                                        });

                                        services.AddSingleton(provider => {
                                            var bookStoreIdentityDbOptions = new DbContextOptionsBuilder<BookStoreIdentityDbContext>()
                                           .UseInMemoryDatabase("IMBookStoreIdentity")
                                           .Options;
                                            var bookStoreIdentityDbContext = new BookStoreIdentityDbContext(bookStoreIdentityDbOptions);
                                            bookStoreIdentityDbContext.Database.EnsureDeleted();
                                            return bookStoreIdentityDbContext;
                                        });

                                        services.AddControllers().AddNewtonsoftJson(options => {
                                            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                                            options.SerializerSettings.MaxDepth = 2;
                                        });
                                        services.AddScoped<IBookStoreUnitOfWorkAsync, MockBookStoreInMemoryUnitOfWork>();
                                        services.AddTransient<IAppDataSeeder, TDataSeeder>();
                                    }));
        }


    }
}
