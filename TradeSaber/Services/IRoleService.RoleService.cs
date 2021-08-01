using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;

        public RoleService(ILogger<RoleService> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }

        public async Task<Role?> CreateRole(string name, string[] scopes, bool root = false)
        {
            Role? role = await GetRole(name);
            if (role is not null)
            {
                _logger.LogInformation("A role by then name {Name} already exists.", name);
                return null;
            }
            role = new Role
            {
                Name = name,
                Root = root,
                Scopes = scopes.ToList(),
            };
            _tradeContext.Roles.Add(role);
            await _tradeContext.SaveChangesAsync();
            return role;
        }

        public async Task<Role?> GetRole(string roleName)
{
            string roleLower = roleName.ToLower();
            Role? role = await _tradeContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == roleLower);
            return role;
        }

        public async Task<Role> GetRootRole()
        {
            Role? role =  await _tradeContext.Roles.FirstOrDefaultAsync(r => r.Root);
            if (role is null)
            {
                role = await CreateRole("Owner", Scopes.AllScopes, true);
            }
            if (role!.Scopes.Count != Scopes.AllScopes.Length)
            {
                role = await SetScopes(role, Scopes.AllScopes);
            }
            return role;
        }

        public async Task<Role> SetScopes(Role role, params string[] newScopes)
        {
            role.Scopes = newScopes.ToList();
            await _tradeContext.SaveChangesAsync();
            return role;
        }
    }
}