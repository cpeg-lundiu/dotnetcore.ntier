using Microsoft.AspNetCore.Mvc;
using DotNetCore.NTier.Services;
using DotNetCore.NTier.Models.Dao;
using Microsoft.AspNetCore.Authorization;

namespace DotNetCore.NTier.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet(Name = "GetUsers")]
        public IAsyncEnumerable<User> Get()
        {
            return _userService.GetUsers();
        }
    }
}