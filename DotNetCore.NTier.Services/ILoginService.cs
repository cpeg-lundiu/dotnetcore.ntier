using DotNetCore.NTier.Models.Dto;

namespace DotNetCore.NTier.Services
{
    public interface ILoginService
    {
        public Task<LoginResponse> Authenticate(LoginRequest model, string jwtSecret, string ipAddress);
    }
}
