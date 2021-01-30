using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;
using TradeSaber.Settings;

namespace TradeSaber.Services
{
    public class DiscordAuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly JWTSettings _jwtSettings;
        private readonly TradeContext _tradeContext;

        public DiscordAuthService(ILogger<DiscordAuthService> logger, JWTSettings jwtSettings, TradeContext tradeContext)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
            _tradeContext = tradeContext;
        }

        public async Task<User?> GetUser(Guid? id)
        {
            return await _tradeContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.ID == id);
        }

        public string Sign(Guid id, float lengthInHours = 1344, params string[] scopes)
        {
            _logger.LogInformation("Signing HMACSHA256 Symmetric Security Key for {id}", id);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim> { new Claim("sub", id.ToString()) };
            if (scopes.Length != 0)
                claims.Add(new Claim("scope", scopes.Aggregate((a, b) => a + " " + b)));
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(lengthInHours),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}