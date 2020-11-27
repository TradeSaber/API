using System;
using NodaTime;
using System.Linq;
using System.Text;
using TradeSaber.Settings;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TradeSaber.Authorization
{
    public class JWTValidator
    {
        private readonly IClock _clock;
        private readonly RequestDelegate _next;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<JWTValidator> _logger;

        public JWTValidator(IClock clock, RequestDelegate next, JWTSettings jwtSettings, ILogger<JWTValidator> logger)
        {
            _next = next;
            _clock = clock;
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        public async Task Invoke(HttpContext context, TradeContext tradeContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").LastOrDefault();
            if (token != null)
                await AttachUserToContext(context, tradeContext, token);
            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, TradeContext tradeContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "sub").Value;
                if (Guid.TryParse(userId, out Guid id))
                {
                    var user = await tradeContext.Users.FirstAsync(u => u.ID == id);
                    context.Items["User"] = user;
                    user.LastLoggedIn = _clock.GetCurrentInstant();
                    await tradeContext.SaveChangesAsync();
                    _logger.LogInformation("User {Username}#{Discriminator} is accessing {DisplayName}.", user.Profile.Username, user.Profile.Discriminator, context.GetEndpoint()?.DisplayName);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error validating login authentication token. {Message}", e.Message);
            }
        }
    }
}