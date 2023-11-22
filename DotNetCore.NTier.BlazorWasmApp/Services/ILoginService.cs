using DotNetCore.NTier.Models.Dto;

namespace DotNetCore.NTier.BlazorWasmApp.Services
{
    public interface ILoginService
    {
        public Task Initialize();
        public Task Login(LoginRequest model);
        public bool IsLoggedIn { get; }
        public Task Logout();
    }
}
