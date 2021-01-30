using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;
using TradeSaber.Models.Discord;
using TradeSaber.Services;
using TradeSaber.Settings;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly DiscordService _discordService;
        private readonly DiscordSettings _discordSettings;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, TradeContext tradeContext, DiscordService discordService, DiscordSettings discordSettings)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
            _discordService = discordService;
            _discordSettings = discordSettings;
        }

        /// <summary>
        /// Redirects to Discord's OAuth2 Flow.
        /// </summary>
        [HttpGet]
        public IActionResult Authenticate()
        {
            return Redirect($"{_discordSettings.URL}/oauth2/authorize?response_type=code&client_id={_discordSettings.ID}&scope=identify&redirect_uri={_discordSettings.RedirectURL}");
        }

        /// <summary>
        /// Callback for Discord's OAuth2 Flow.
        /// </summary>
        /// <param name="code">The user access code from authorization flow.</param>
        [HttpGet("callback")]
        public async Task<ActionResult<TokenContainer>> Callback([FromQuery] string code)
        {
            string? accessToken = await _discordService.GetAccessToken(code);
            // Invalid Code Vibe Check
            if (accessToken is null)
                return NotFound(Error.Create("Could not validate code."));

            DiscordUser? profile = await _discordService.GetProfile(accessToken);
            // Shouldn't really happen... unless discord servers JUST STARTED-
            // to have issues or we're rate limited.
            if (profile is null)
                return NotFound(Error.Create("Could not fetch user profile from Discord."));

            User? user = await _tradeContext.Users.FirstOrDefaultAsync(u => u.Profile.Id == profile.Id);
            // User is not in our database?
            if (user is null)
            {
                user = new User
                {
                    ID = Guid.NewGuid(),
                    Profile = profile
                };
                _logger.LogInformation("Creating a new user. {Username}#{Discriminator}", user.Profile.Username, user.Profile.Discriminator);
                await _tradeContext.Users.AddAsync(user);
            }
            else user.Profile = profile;
            await _tradeContext.SaveChangesAsync();

            string token = _authService.Sign(user.ID);
            return Ok(new TokenContainer { Token = token });
        }

        [Authorize]
        [HttpGet("@me")]
        public async Task<ActionResult<User>> GetSelf()
        {
            // Although GetUser can return null, if they made it this far into the request it shouldn't return null unless they were manually deleted off the database and their key remained.
            return Ok(await _authService.GetUser(User.GetID()));
        }

        public class TokenContainer
        {
            public string Token { get; set; } = null!;
        }
    }
}