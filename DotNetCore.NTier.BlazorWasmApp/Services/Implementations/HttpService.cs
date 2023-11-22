using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using DotNetCore.NTier.Models.Dto;

namespace DotNetCore.NTier.BlazorWasmApp.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IJSRuntime _jsRuntime;

        public HttpService(
            HttpClient httpClient,
            ILocalStorageService localStorageService,
            AuthenticationStateProvider authStateProvider,
            IJSRuntime jsRuntime
        )
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authStateProvider = authStateProvider;
            _jsRuntime = jsRuntime;
        }

        public async Task<T?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequest<T>(request);
        }

        public async Task Post(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            await SendRequest(request);
        }

        public async Task<T?> Post<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Post, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Put(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            await SendRequest(request);
        }

        public async Task<T?> Put<T>(string uri, object value)
        {
            var request = CreateRequest(HttpMethod.Put, uri, value);
            return await SendRequest<T>(request);
        }

        public async Task Delete(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            await SendRequest(request);
        }

        public async Task<T?> Delete<T>(string uri)
        {
            var request = CreateRequest(HttpMethod.Delete, uri);
            return await SendRequest<T>(request);
        }

        // helper methods

        private static HttpRequestMessage CreateRequest(HttpMethod method, string uri, object? value = null)
        {
            var request = new HttpRequestMessage(method, uri);

            if (value != null)
            {
                if (value.GetType() == typeof(MultipartFormDataContent))
                {
                    request.Content = (MultipartFormDataContent)value;
                }
                else
                {
                    request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
                }
            }

            return request;
        }

        private async Task SendRequest(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorageService.RemoveItem("loginCred");
                ((AuthStateProvider)_authStateProvider).NotifyUserLogOut();
                return;
            }

            await HandleErrors(response);

            if (request.Content?.GetType() == typeof(MultipartFormDataContent) && response.IsSuccessStatusCode)
            {
                await _jsRuntime.InvokeVoidAsync("alert", "Upload success.");
            }
        }

        private async Task<T?> SendRequest<T>(HttpRequestMessage request)
        {
            await AddJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorageService.RemoveItem("loginCred");
                ((AuthStateProvider)_authStateProvider).NotifyUserLogOut();
                return default;
            }

            await HandleErrors(response);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return default;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var type = typeof(T);

            if (type == typeof(string))
            {
                var result = await response.Content.ReadAsStringAsync();
                return (T)Convert.ChangeType(result, typeof(T));
            }

            if (type == typeof(byte[]))
            {
                var result = await response.Content.ReadAsStreamAsync();
                var ms = new MemoryStream();
                result.CopyTo(ms);
                return (T)Convert.ChangeType(ms.ToArray(), typeof(T));
            }
            else
            {
                if (response != null && response.Content.Headers.Any())
                    return await response.Content.ReadFromJsonAsync<T>(options);
                else
                    return default;
            }
        }

        private async Task AddJwtHeader(HttpRequestMessage request)
        {
            try
            {
                // add jwt auth header if user is logged in and request is to the api url
                var user = await _localStorageService.GetItem<LoginResponse>("loginCred");
                var isApiUrl = !request.RequestUri.IsAbsoluteUri;
                if (user != null && isApiUrl)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.JwtToken);
            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        private async Task HandleErrors(HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                // custom application error message on status code 409
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _jsRuntime.InvokeVoidAsync("alert", errorMessage);
                    return;
                }

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

                if (error != null)
                    throw new Exception(error["message"]);

                return;
            }
        }
    }
}
