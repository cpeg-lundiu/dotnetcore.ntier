using Microsoft.EntityFrameworkCore;
using DotNetCore.NTier.Models.Dto;

namespace DotNetCore.NTier.Services
{
    public class LoginService : JwtTokenUtil, ILoginService
    {
        private readonly AppDataContext _context;

        public LoginService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse> Authenticate(LoginRequest model, string jwtSecret, string ipAddress)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(o =>
                        o.Username == model.Username &&
                        o.Password == EncryptionUtil.HashString(model.Username + model.Password));

            // return null if member not found
            if (user == null)
            {
                throw new ApplicationException("Username or password is incorrect");
            }

            // authentication successful so generate jwt token
            var jwtToken = GenerateJwtToken(user, jwtSecret);

            return new LoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                JwtToken = jwtToken
            };
        }
    }
}
