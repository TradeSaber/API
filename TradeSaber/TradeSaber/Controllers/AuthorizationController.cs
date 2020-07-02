using TradeSaber.Models;
using TradeSaber.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TradeSaber.Controllers
{
    [Route("authorize")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly UserService _userService;
        private readonly DiscordService _discordService;

        public AuthorizationController(JWTService jwt, UserService user, DiscordService discord)
        {
            _jwtService = jwt;
            _userService = user;
            _discordService = discord;
        }

        [HttpGet]
        public IActionResult Authenticate() =>
            Redirect($"https://discordapp.com/api/oauth2/authorize?response_type=code&client_id={_discordService._id}&scope=identify&redirect_uri={_discordService._redirectURL}");

        [HttpGet("callback")]
        public async Task<IActionResult> Authed([FromQuery(Name = "code")] string code)
        {
            try
            {
                // Load all the discord related profile information
                string accessCode = await _discordService.SendDiscordOAuthRequestViaAuthCode(code);
                DiscordUser discord = await _discordService.GetDiscordUserProfile(accessCode);
                User user = _userService.Get(discord.ID);
                if (user == null)
                {
                    // If the user doesn't exist, create it and add it to the database.
                    user = new User
                    {
                        Banned = false,
                        DiscordID = discord.ID,
                        Role = TradeSaberRole.None,
                        Inventory = new List<string>(),
                        UnopenedPacks = new List<string>()
                    };
                    _userService.Create(user);
                }
                user.Profile = discord;
                _userService.Update(user);

                // Generate the JWT token to ship off to the user
                string token = _jwtService.GenerateUserToken(user, 24 * 30 * 12);
                return Ok(new { token, user });
            }
            catch
            {
                return Problem();
            }
        }

        [HttpPost("game")]
        public async Task<IActionResult> GameLogin([FromBody] GameLoginBody body)
        {
            try
            {
                DiscordUser discord = await _discordService.GetDiscordUserProfile(body.AccessToken);
                User user = _userService.Get(discord.ID);
                if (user == null)
                {
                    // If the user doesn't exist, create it and add it to the database.
                    user = new User
                    {
                        Banned = false,
                        DiscordID = discord.ID,
                        Role = TradeSaberRole.None,
                        Inventory = new List<string>(),
                        UnopenedPacks = new List<string>()
                    };
                    _userService.Create(user);
                }
                user.Profile = discord;
                _userService.Update(user);

                // Generate the JWT token to ship off to the user
                string token = _jwtService.GenerateUserToken(user, 6);
                return Ok(new { token, user });
            }
            catch
            {
                return Problem();
            }
        }

        public class GameLoginBody
        {
            public string AccessToken { get; set; }
        }
    }
}
