using BookStoreApi.Code.DataContoroller.Entity;
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
using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using BookStoreApi.Data.DTOs;
using Newtonsoft.Json;
using System.Net.Mime;

namespace BookStoreApiTests.TestServers {
    public static class AuthorizeMethods {
        public static Task Autorize(Func<UserLoginDTO> login, HttpClient client) {
            return Task.Run(() => {
                var dtoString = JsonConvert.SerializeObject(login.Invoke());
                var message = new HttpRequestMessage(HttpMethod.Post, "api/Users/login") {
                    Content = new StringContent(dtoString, Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                var responce = client.SendAsync(message).Result;
                if (responce.IsSuccessStatusCode) {
                    var loginData = JsonConvert.DeserializeObject<UserLoginData>(responce.Content.ReadAsStringAsync().Result);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginData.Token);
                }
            });
        }

        public static async Task AutorizeAsync(Func<UserLoginDTO> login, HttpClient client) {
            var dtoString = JsonConvert.SerializeObject(login.Invoke());
            var message = new HttpRequestMessage(HttpMethod.Post, "api/Users/login") {
                Content = new StringContent(dtoString, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var responce = await client.SendAsync(message);
            if (responce.IsSuccessStatusCode) {
                var loginData = JsonConvert.DeserializeObject<UserLoginData>(await responce.Content.ReadAsStringAsync());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginData.Token);
            }
        }
    }
}
