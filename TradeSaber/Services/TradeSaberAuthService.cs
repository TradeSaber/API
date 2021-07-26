using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TradeSaber.Authorization;
using TradeSaber.Settings;

namespace TradeSaber.Services
{
    public class TradeSaberAuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly JWTSettings _jwtSettings;

        public TradeSaberAuthService(ILogger<TradeSaberAuthService> logger, JWTSettings jwtSettings)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        public string Sign(Guid id, float lengthInHours = 4, params string[] scopes)
        {
            _logger.LogInformation("Signing HMACSHA256 Symmetric Security Key for {id}", id);
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new() { new Claim("sub", id.ToString()) };
            if (scopes.Length != 0)
                claims.Add(new Claim("scope", scopes.Aggregate((a, b) => a + " " + b)));
            JwtSecurityToken token = new(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(lengthInHours),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}