using Microsoft.AspNetCore.Mvc;
using DotNetCore.NTier.Models.Dto;
using DotNetCore.NTier.Services;
using Microsoft.Extensions.Options;
using DotNetCore.NTier.WebApp.Models;
using System.Diagnostics;

namespace DotNetCore.NTier.WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly AppSettings _appSettings;

        public LoginController(ILoginService loginService, IOptions<AppSettings> appSettings)
        {
            _loginService = loginService;
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _loginService.Authenticate(loginRequest, _appSettings.UserJwtSecret, IpAddress());

                    if (response == null)
                        return RedirectToAction("Error");


                    SetTokenCookie(string.IsNullOrEmpty(response.JwtToken) ? string.Empty : response.JwtToken);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Username", ex.Message);
                    return View(loginRequest);
                }
            }
            else
            {
                return View(loginRequest);
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            Response.Cookies.Append("jwtToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"].ToString();
            else
                return HttpContext.Connection.RemoteIpAddress != null ? HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() : string.Empty;
        }
    }
}
