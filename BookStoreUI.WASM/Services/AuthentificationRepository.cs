using Blazored.LocalStorage;
using BookStoreUI.WASM.Contracts;
using BookStoreUI.WASM.Data;
using BookStoreUI.WASM.Data.DTOs;
using BookStoreUI.WASM.Data.Models;
using BookStoreUI.WASM.Data.ViewModels;
using BookStoreUI.WASM.Data.ViewModels.Authentification;
using BookStoreUI.WASM.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Services {
    public class AuthentificationRepository : Contracts.IAuthentificationRepository {
        private readonly string _ApiUrl;
        private readonly HttpClient _Client;
        private readonly ILocalStorageService _LocalStorage;
        private readonly AuthenticationStateProvider _AuthenticationStateProvider;
        private readonly IConfiguration _Configuration;

        public AuthentificationRepository(IConfiguration configuration,
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider) {
            _Configuration = configuration;
            _Client = httpClient;
            _ApiUrl = configuration.GetValue<string>(ConventionalUrls.BaseUrlConfigurationKey);
            _LocalStorage = localStorage;
            _AuthenticationStateProvider = authenticationStateProvider;
        }



        public async Task<RepositoryResponce> Register(RegistrationVM userData) {
            if (userData == null)
                return RepositoryResponce.ArgumentNullResponce;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string url = Flurl.Url.Combine(_ApiUrl, ConventionalUrls.RegisterRelativeUrl);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var responce = await _Client.SendAsync(httpRequestMessage);
            return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
        }

        public async Task<RepositoryResponce> Login(UserLoginDTO userData) {
            if (userData == null)
                return RepositoryResponce.ArgumentNullResponce;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string url = Flurl.Url.Combine(_ApiUrl, ConventionalUrls.LoginRelativeUrl);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url) {
                Content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            var responce = await _Client.SendAsync(httpRequestMessage);
            if (!responce.IsSuccessStatusCode)
                return RepositoryResponce.StatusCodeResponce(responce.StatusCode);
            else {
                var loginData = JsonConvert.DeserializeObject<Data.DTOs.SessionDataObject>(await responce.Content.ReadAsStringAsync());
                await _LocalStorage.SetItemAsync(ConventionalKeys.TokenStorageKey, loginData.Token);
                await ((ApiAuthentificationStateProvider)_AuthenticationStateProvider).LoggedIn();
                _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "bearer", loginData.Token);
                var reponse = RepositoryResponce.StatusCodeResponce(responce.StatusCode);
                reponse.Message = loginData.Answer;
                return reponse;
            }
        }

        public async Task<RepositoryResponce> ChangePassword(ChangePasswordDTO changePasswordDto) {
            if (changePasswordDto == null)
                return RepositoryResponce.ArgumentNullResponce;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string url = Flurl.Url.Combine(_ApiUrl, ConventionalUrls.ChangePasswordRelativeUrl);
            try {
                _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    scheme: "bearer",
                    parameter: await _LocalStorage.GetItemAsStringAsync(ConventionalKeys.TokenStorageKey)
                    );
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url) {
                    Content = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, MediaTypeNames.Application.Json)
                };
                var responce = await _Client.SendAsync(httpRequestMessage);
                PasswordActionAnswer answer = JsonConvert.DeserializeObject<PasswordActionAnswer>(await responce.Content.ReadAsStringAsync());

                var repositoryReponce = RepositoryResponce.StatusCodeResponce(responce.StatusCode);
                repositoryReponce.Message = answer.ServerMessage;
                return repositoryReponce;
            }
            catch(Exception e) {
                return new RepositoryResponce() { Succeed = false, Errror = e.Message };
            }
        }

        public async Task Logout() {
            await ((ApiAuthentificationStateProvider)_AuthenticationStateProvider).LoggedOut();
        }
    }
}
