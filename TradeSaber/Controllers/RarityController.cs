using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RarityController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;
        
        public RarityController(ILogger<RarityController> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }

        [HttpGet]
        public IAsyncEnumerable<Rarity> GetAllRarities()
        {
            return _tradeContext.Rarities.AsAsyncEnumerable();
        }

        [HttpPost]
        [Authorize(Scopes.CreateRarity)]
        public async Task<ActionResult<Rarity>> CreateRarity([FromBody] CreateRarityBody body)
        {
            Rarity? rarity = await _tradeContext.Rarities.FirstOrDefaultAsync(r => r.Name.ToLower() == body.Name.ToLower());
            if (rarity is not null)
            {
                return BadRequest(Error.Create("Rarity already exists."));
            }
            _logger.LogInformation("Creating new rarity. {Name}", body.Name);
            rarity = new Rarity
            {
                Name = body.Name,
                Color = body.Color,
                Probability = body.Probability,
                ID = Guid.NewGuid()
            };
            _tradeContext.Rarities.Add(rarity);
            await _tradeContext.SaveChangesAsync();
            return Ok(rarity);
        }
        
        [HttpPatch]
        [Authorize(Scopes.ManageRarity)]
        public async Task<ActionResult<Rarity>> EditRarity([FromBody] EditRarityBody body)
        {
            Rarity? rarity = await _tradeContext.Rarities.FirstOrDefaultAsync(r => r.Name.ToLower() == body.Name.ToLower());
            if (rarity is null)
            {
                return NotFound(Error.Create("Unknown rarity."));
            }
            _logger.LogInformation("Editing rarity. {Name}", body.Name);

            if (body.Color is not null)
                rarity.Color = body.Color;

            if (body.Probability is not null)
                rarity.Probability = body.Probability.Value;

            await _tradeContext.SaveChangesAsync();
            return Ok(rarity);
        }

        public class CreateRarityBody
        {
            public string Name { get; set; } = null!;
            public string Color { get; set; } = null!;
            public float Probability { get; set; }
        }

        public class EditRarityBody
        {
            public string Name { get; set; } = null!;
            public string? Color { get; set; }
            public float? Probability { get; set; }
        }
    }
}