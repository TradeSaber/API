using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly TradeContext _tradeContext;

        public LeaderboardController(TradeContext tradeContext)
        {
            _tradeContext = tradeContext;
        }

        [HttpGet("tir/{page}")]
        public async Task<ActionResult<IEnumerable<RankedUser>>> GetRankedTir(int page = 1)
        {
            const int pageSize = 10;
            if (page <= 0)
                page = 1;
            page--;
            var users = (await _tradeContext.AllUsersByTir()).Select(u => new RankedUser(u));
            return Ok(users.Skip(page * pageSize).Take(pageSize));
        }

        [HttpGet("portfolio/{page}")]
        public async Task<ActionResult<IEnumerable<RankedUser>>> GetRankedPortfolios(int page = 1)
        {
            const int pageSize = 10;
            if (page <= 0)
                page = 1;
            page--;
            var users = (await _tradeContext.AllUsersByPortfolio()).Select(u => new RankedUser(u));
            return Ok(users.Skip(page * pageSize).Take(pageSize));
        }

        [HttpGet("total/{page}")]
        public async Task<ActionResult<IEnumerable<RankedUser>>> GetRankedTotal(int page = 1)
        {
            const int pageSize = 10;
            if (page <= 0)
                page = 1;
            page--;
            var users = (await _tradeContext.AllUsersByTotal()).Select(u => new RankedUser(u));
            return Ok(users.Skip(page * pageSize).Take(pageSize));
        }

        public class RankedUser
        {
            public User User { get; set; } = null!;
            public float TirCoin => User.Inventory.TirCoin;
            public int Rank => User.Inventory.Rank.GetValueOrDefault();
            public float PortfolioValue => User.Inventory.PortfolioValue.GetValueOrDefault();

            public RankedUser(User user)
            {
                User = user;
            }
        }
    }
}