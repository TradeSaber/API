using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly RewardService _rewardService;

        public ShopController(ILogger<ShopController> logger, IAuthService authService, TradeContext tradeContext, RewardService rewardService)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
            _rewardService = rewardService;
        }

        [HttpGet]
        public IAsyncEnumerable<Pack> AvailableToBuy()
        {
            return _tradeContext.Packs.Include(p => p.Cover).Include(p => p.Rarities).Where(p => p.Value.HasValue).AsAsyncEnumerable();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Buy([FromBody] BuyBody body)
        {
            Pack? pack = await _tradeContext.Packs.FirstOrDefaultAsync(p => p.ID == body.PackID);
            if (pack is null)
            {
                return NotFound(Error.Create("Could not find pack."));
            }
            if (!pack.Value.HasValue)
            {
                return BadRequest(Error.Create("Pack is not for sale."));
            }
            int quantity = body.Quantity ?? 1;
            if (quantity <= 0)
                quantity = 1;
            float cost = pack.Value.Value * quantity;
            User user = (await _authService.GetUser(User.GetID()))!;
            if (cost > user.Inventory.TirCoin)
            {
                return BadRequest(Error.Create("Cannot afford pack."));
            }
            user.Inventory.TirCoin -= cost;
            for (int i = 0; i < quantity; i++)
            {
                user.Inventory.Packs.Add(new Pack.Reference { Pack = pack });
            }
            _tradeContext.Users.Update(user);
            return NoContent();
        }

        public class BuyBody
        {
            public Guid PackID { get; set; }
            public int? Quantity { get; set; }

        }
    }
}