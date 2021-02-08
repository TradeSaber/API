using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public class RewardService
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;

        public RewardService(ILogger<RewardService> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }

        public async Task AddXP(User user, float amount)
        {
            _logger.LogInformation("Adding {amount} XP to {Username}.", amount, user.Profile.Username);
            user.XP += (await XPBoostValue()) * amount;
            _tradeContext.Users.Update(user);
        }

        public async Task AddTir(User user, float amount)
        {
            _logger.LogInformation("Adding T{amount} to {Username}.", amount, user.Profile.Username);
            user.Inventory.TirCoin += (await TirBoostValue()) * amount;
            _tradeContext.Users.Update(user);
        }

        private async Task<float> XPBoostValue()
        {
            IEnumerable<Mutation> activeMutations = await _tradeContext.Mutations.Where(m => m.Active && m.GlobalXPBoost != null).ToListAsync();
            float defaultValue = 1f;
            foreach (var mutation in activeMutations)
            {
                defaultValue *= mutation.GlobalXPBoost!.Value;
            }
            return defaultValue;
        }

        private async Task<float> TirBoostValue()
        {
            IEnumerable<Mutation> activeMutations = await _tradeContext.Mutations.Where(m => m.Active && m.GlobalTirBoost != null).ToListAsync();
            float defaultValue = 1f;
            foreach (var mutation in activeMutations)
            {
                defaultValue *= mutation.GlobalTirBoost!.Value;
            }
            return defaultValue;
        }
    }
}