using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly SoriginAuthorizer _soriginAuthorizer;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IUserService userService, SoriginAuthorizer soriginAuthorizer)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
            _soriginAuthorizer = soriginAuthorizer;
        }

        [Authorize]
        [HttpGet("@me")]
        public async Task<ActionResult<User>> Self()
        {
            return Ok(await _userService.GetUser(User.GetID().GetValueOrDefault(), true));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginBody body)
        {
            _logger.LogInformation("Authorizing via Sorigin.");
            SoriginUser? soriginUser = await _soriginAuthorizer.Authorize(body.Token);
            if (soriginUser is null)
            {
                return Unauthorized(Error.Create("Could not authorize through Sorigin."));
            }
            User user = await _userService.CreateNewUser(soriginUser.ID);
            string token = _authService.Sign(user.ID, 4, user.Role?.Scopes.ToArray() ?? Array.Empty<string>());
            return Ok(new { token });
        }

        public class LoginBody
        {
            public string Token { get; set; } = null!;
        }
    }
}