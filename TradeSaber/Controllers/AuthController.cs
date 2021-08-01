using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TradeSaber.Models;
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly SoriginAuthorizer _soriginAuthorizer;
        private readonly TradeSaberAuthService _tradeSaberAuthService;


        public AuthController(ILogger<AuthController> logger, SoriginAuthorizer soriginAuthorizer, TradeSaberAuthService tradeSaberAuthService)
        {
            _logger = logger;
            _soriginAuthorizer = soriginAuthorizer;
            _tradeSaberAuthService = tradeSaberAuthService;
        }

        public async Task<ActionResult> Login([FromBody] LoginBody body)
        {
            _logger.LogInformation("Authorizing via Sorigin.");
            SoriginUser? soriginUser = await _soriginAuthorizer.Authorize(body.Token);
            if (soriginUser is null)
            {
                return Unauthorized(Error.Create("Could not authorize through Sorigin."));
            }
            throw new System.NotImplementedException();
        }

        public class LoginBody
        {
            public string Token { get; set; } = null!;
        }
    }
}