using System;
using System.Text;
using TradeSaber.Models;
using System.Security.Claims;
using TradeSaber.Models.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TradeSaber.Services
{
    public class JWTService
    {
        private readonly string _key;
        private readonly string _issuer;

        public JWTService(IJWTSettings settings)
        {
            _key = settings.Key;
            _issuer = settings.Issuer;
        }

        public string GenerateUserToken(User user, float timeInHours = 24f)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.DiscordID),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims,
                expires: DateTime.UtcNow.AddHours(timeInHours),
                signingCredentials: credentials);

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }
    }
}
