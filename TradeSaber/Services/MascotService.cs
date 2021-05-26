using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSaber.Models;
using TradeSaber.Models.Discord;
using TradeSaber.Settings;

namespace TradeSaber.Services
{
    public class MascotService
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;
        private readonly DiscordService _discordService;
        private readonly DiscordSettings _discordSettings;

        public MascotService(ILogger<MascotService> logger, TradeContext tradeContext, DiscordService discordService, DiscordSettings discordSettings)
        {
            _logger = logger;
            _tradeContext = tradeContext;
            _discordService = discordService;
            _discordSettings = discordSettings;
        }

        public async Task<User> GetMascot()
        {
            var id = _discordSettings.MascotID;
            User? user = await _tradeContext.Users.Include(u => u.Role).Include(u => u.Inventory).FirstOrDefaultAsync(u => u.Profile.Id == id);
            if (user is null)
            {
                _logger.LogInformation($"Generating Mascot...");
                DiscordUser profile = (await _discordService.GetProfileFromID(id))!;
                Role? role = await _tradeContext.Roles.FirstOrDefaultAsync(r => r.Name == profile.Username);
                if (role is null)
                {
                    role = new Role
                    {
                        ID = Guid.NewGuid(),
                        Name = profile.Username,
                        Scopes = new List<string> { Scopes.Mascot }
                    };
                    _logger.LogInformation($"Generating {profile.Username} Role");
                    _tradeContext.Roles.Add(role);
                }
                var guid = Guid.NewGuid();
                user = new User
                {
                    ID = guid,
                    Profile = profile,
                    Role = role,
                    Inventory = new Inventory
                    {
                        ID = guid
                    }
                };
                _tradeContext.Inventories.Add(user.Inventory);
                _tradeContext.Users.Add(user);
                await _tradeContext.SaveChangesAsync();
            }
            return user;
        }
    }
}