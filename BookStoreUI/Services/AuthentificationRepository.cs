using Blazored.LocalStorage;
using BookStoreUI.Data;
using BookStoreUI.Data.DTOs;
using BookStoreUI.Data.Models;
using BookStoreUI.Data.ViewModels;
using BookStoreUI.Data.ViewModels.Authentification;
using BookStoreUI.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.Services {
    public class AuthentificationRepository : Contracts.IAuthentificationRepository {
        private readonly string apiUrl;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly ILocalStorageService _LocalStorage;
        private readonly AuthenticationStateProvider _AuthenticationStateProvider;

        public AuthentificationRepository(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider) {
            _ClientFactory = httpClientFactory;
            apiUrl = configuration.GetValue<string>(ConventionalUrls.BaseUrlConfigurationKey);
            _LocalStorage = localStorage;
            _AuthenticationStateProvider = authenticationStateProvider;
        }



        public async Task<RepositoryResponce> Register(RegistrationVM userData) {
            if (userData == null)
                return RepositoryResponce.ArgumentNullResponce;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            using var client = _ClientFactory.CreateClient();
            string url = Flurl.Url.Combine(apiUrl, ConventionalUrls.RegisterRelativeUrl);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var responce = await client.SendAsync(httpRequestMessage);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }

        public async Task<RepositoryResponce> Login(UserLoginDTO userData) {
            if (userData == null)
                return RepositoryResponce.ArgumentNullResponce;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            using var client = _ClientFactory.CreateClient();
            string url = Flurl.Url.Combine(apiUrl, ConventionalUrls.LoginRelativeUrl);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var responce = await client.SendAsync(httpRequestMessage);
            if(!responce.IsSuccessStatusCode)
                return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
            else {
                var loginData = JsonConvert.DeserializeObject<Data.DTOs.SessionDataObject>(await responce.Content.ReadAsStringAsync());
                await _LocalStorage.SetItemAsync(ConventionalKeys.TokenStorageKey, loginData.Token);
                await ((ApiAuthentificationStateProvider)_AuthenticationStateProvider).LoggedIn();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "bearer", loginData.Token); 

                var reponse = RepositoryResponce.StatusCodeResponce(responce.StatusCode);
                reponse.Message = loginData.Answer;
                return reponse;
            }
        }

        public async Task Logout() {
            await ((ApiAuthentificationStateProvider)_AuthenticationStateProvider).LoggedOut();
        }
    }
}
