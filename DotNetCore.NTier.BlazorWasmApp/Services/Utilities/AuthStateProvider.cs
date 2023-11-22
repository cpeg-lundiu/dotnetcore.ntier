using DotNetCore.NTier.Models.Dto;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DotNetCore.NTier.BlazorWasmApp.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState _anonymous;
        private readonly ILocalStorageService _localStorageService;

        public AuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorageService = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItem<LoginResponse>("loginCred");

            if (token == null)
            {
                return _anonymous;
            }

            var claims = JwtParser.ParseClaimsFromJwt(token.JwtToken.ToString());

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        public void NotifyUserAuthentication(string token)
        {
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogOut()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
