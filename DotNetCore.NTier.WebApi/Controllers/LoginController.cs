using DotNetCore.NTier.Models.Dto;
using DotNetCore.NTier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DotNetCore.NTier.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly AppSettings _appSettings;

        public LoginController(ILoginService loginService, IOptions<AppSettings> appSettings)
        {
            _loginService = loginService;
            _appSettings = appSettings.Value;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateByUsernameAndPassword([FromBody] LoginRequest model)
        {
            var response = await _loginService.Authenticate(model, _appSettings.UserJwtSecret, IpAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        // helper methods

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"].ToString();
            else
                return HttpContext.Connection.RemoteIpAddress != null ? HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() : string.Empty;
        }
    }
}