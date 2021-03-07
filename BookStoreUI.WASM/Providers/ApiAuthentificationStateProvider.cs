using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Providers {
    public class ApiAuthentificationStateProvider : AuthenticationStateProvider {

        private readonly ILocalStorageService _LocalStorage;
        private readonly JwtSecurityTokenHandler _TokenHandler;
        public ApiAuthentificationStateProvider(ILocalStorageService localStorage,
            JwtSecurityTokenHandler tokenHandler) {
            _LocalStorage = localStorage;
            _TokenHandler = tokenHandler;

        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            try {
                if (!await _LocalStorage.ContainKeyAsync(Data.ConventionalKeys.TokenStorageKey))
                    return new AuthenticationState(new ClaimsPrincipal());
                string tokenString = await _LocalStorage.GetItemAsync<string>(Data.ConventionalKeys.TokenStorageKey);
                if (!_TokenHandler.CanReadToken(tokenString)) {
                    return await AnonymousState();
                }
                var token = _TokenHandler.ReadJwtToken(tokenString);
                if (token.ValidTo.CompareTo(DateTime.Now) < 0)
                    return await AnonymousState();

                var claims = GetClaims(token);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                return new AuthenticationState(user);
            }
            catch (Exception) {

                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        public async Task LoggedIn() {
            string tokenString = await _LocalStorage.GetItemAsync<string>(Data.ConventionalKeys.TokenStorageKey);
            var token = _TokenHandler.ReadJwtToken(tokenString);
            var claims = GetClaims(token);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task LoggedOut() {
            NotifyAuthenticationStateChanged(Task.FromResult(await AnonymousState()));
        }

        private async Task<AuthenticationState> AnonymousState() {
            try {
                if (await _LocalStorage.ContainKeyAsync(Data.ConventionalKeys.TokenStorageKey))
                    await _LocalStorage.RemoveItemAsync(Data.ConventionalKeys.TokenStorageKey);
            }
            finally {
                
            }
            return new AuthenticationState(new ClaimsPrincipal());
        }

        private static IList<Claim>  GetClaims(JwtSecurityToken tokenContent) {
            var result = tokenContent.Claims.ToList();
            result.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return result;
        }
    }
}
