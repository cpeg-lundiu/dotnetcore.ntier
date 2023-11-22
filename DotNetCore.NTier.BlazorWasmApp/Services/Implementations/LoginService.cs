using DotNetCore.NTier.Models.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace DotNetCore.NTier.BlazorWasmApp.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IHttpService _httpService;
        private readonly AuthenticationStateProvider _authStateProvider;

        public bool IsLoggedIn { get; private set; }

        public async Task Initialize()
        {
            var user = await _localStorageService.GetItem<LoginResponse>("loginCred");

            if (null == user)
            {
                IsLoggedIn = false;
            }
            else
            {
                IsLoggedIn = true;
            }
        }

        public LoginService(ILocalStorageService localStorageService, IHttpService httpService, AuthenticationStateProvider authenticationStateProvider)
        {
            _localStorageService = localStorageService;
            _httpService = httpService;
            _authStateProvider = authenticationStateProvider;
        }

        public async Task Login(LoginRequest model)
        {
            var loginCred = await _httpService.Post<LoginResponse>("login/authenticate", model);

            if (null != loginCred)
            {
                await _localStorageService.SetItem("loginCred", loginCred);
                var token = await _localStorageService.GetItem<LoginResponse>("loginCred");
                var jwt = token.JwtToken.ToString();
                ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(jwt);
                IsLoggedIn = true;
            }
            else
            {
                IsLoggedIn = false;
            }
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItem("loginCred");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogOut();
            IsLoggedIn = false;
        }
    }
}
