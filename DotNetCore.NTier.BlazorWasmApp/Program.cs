using DotNetCore.NTier.BlazorWasmApp;
using DotNetCore.NTier.BlazorWasmApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var apiUrl = builder.Configuration["apiUrl"];
    return apiUrl != null ? new HttpClient() { BaseAddress = new Uri(apiUrl) } : new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
});

builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

await builder.Build().RunAsync();
