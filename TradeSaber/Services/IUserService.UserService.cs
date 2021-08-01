using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TradeSaber.Models;
using TradeSaber.Settings;

namespace TradeSaber.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IRoleService _roleService;
        private readonly MainSettings _mainSettings;
        private readonly TradeContext _tradeContext;

        public UserService(ILogger<UserService> logger, IRoleService roleService, MainSettings mainSettings, TradeContext tradeContext)
        {
            _logger = logger;
            _roleService = roleService;
            _mainSettings = mainSettings;
            _tradeContext = tradeContext;
        }

        public async Task<User?> GetUser(Guid id, bool full = false)
        {
            _logger.LogInformation("Getting a user with the ID {ID} from the database.", id);
            return await _tradeContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task<User> CreateNewUser(Guid id)
        {
            User? user = await GetUser(id);
            if (user is null)
            {
                user = new User
                {
                    ID = id
                };
                _tradeContext.Users.Add(user);
                await _tradeContext.SaveChangesAsync();
            }
            if (user.ID == _mainSettings.Root)
            {
                bool wasNotAssigned = user.Role is null;
                _logger.LogInformation("Root user detected. Assigning root role.");
                user.Role = await _roleService.GetRootRole();
                if (wasNotAssigned)
                    await _tradeContext.SaveChangesAsync();
            }
            return user;
        }
    }
}