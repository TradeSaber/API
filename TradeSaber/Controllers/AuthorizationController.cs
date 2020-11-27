using System;
using NodaTime;
using System.Text;
using TradeSaber.Models;
using TradeSaber.Services;
using TradeSaber.Settings;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradeSaber.Authorization;
using TradeSaber.Models.Discord;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IClock _clock;
        private readonly JWTSettings _jwtSettings;
        private readonly TradeContext _tradeContext;
        private readonly DiscordService _discordService;
        private readonly DiscordSettings _discordSettings;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(IClock clock, JWTSettings jwtSettings, TradeContext tradeContext, DiscordService discordService, DiscordSettings discordSettings, ILogger<AuthorizationController> logger)
        {
            _clock = clock;
            _logger = logger;
            _jwtSettings = jwtSettings;
            _tradeContext = tradeContext;
            _discordService = discordService;
            _discordSettings = discordSettings;
        }

        [HttpGet]
        public IActionResult Authenticate()
        {
            return Redirect($"{_discordSettings.URL}/oauth2/authorize?response_type=code&client_id={_discordSettings.ID}&scope=identify&redirect_uri={_discordSettings.RedirectURL}");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            string? accessToken = await _discordService.GetAccessToken(code);
            
            // Most likely an invalid code.
            if (accessToken == null)
            {
                return NotFound();
            }

            DiscordUser? profile = await _discordService.GetProfile(accessToken);
        
            // Not sure what could have happened here...
            // Rate Limited or discord server issues...
            if (profile == null)
            {
                return NotFound();
            }

            // Look for the user in the database
            var user = await _tradeContext.Users.FirstOrDefaultAsync(u => u.Profile.Id == profile.Id);

            if (user is null)
            {
                // Create the user if there is no user.
                user = new User
                {
                    ID = Guid.NewGuid(),
                    Profile = profile,
                    Created = _clock.GetCurrentInstant(),
                    LastLoggedIn = _clock.GetCurrentInstant(),
                    Level = 1,
                    TirCoin = 0,
                    Experience = 0,
                    Role = Role.None,
                    State = UserState.Active
                };
                _logger.LogInformation("Creating a new user! {Username}#{Discriminator}", user.Profile.Username, user.Profile.Discriminator);

                await _tradeContext.Users.AddAsync(user);
            }
            else
            {
                user.Profile = profile;
            }
            await _tradeContext.SaveChangesAsync();

            // Generate Security Keys
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
            };

            var secToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(288f),
                signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(secToken);

            return Ok(new { token });
        }

        [Tauth]
        [HttpGet("@me")]
        public IActionResult GetSelf()
        {
            if (HttpContext.Items["User"] is not User user)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
