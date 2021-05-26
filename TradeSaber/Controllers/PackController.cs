using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class PackController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;

        public PackController(ILogger<PackController> logger, IAuthService authService, TradeContext tradeContext)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
        }

        [HttpGet]
        public IAsyncEnumerable<Pack> GetAllPacks()
        {
            return _tradeContext.Packs.Include(p => p.Cover).Include(c => c.Cards).Include(c => c.Rarities).ThenInclude(r => r.Rarity).Include(c => c.CardPool).ThenInclude(cp => cp.Card).AsAsyncEnumerable();
        }

        [HttpGet("in-inventory/@me")]
        public async Task<ActionResult<IEnumerable<Pack>>> InSelfInventory()
        {
            User user = (await _authService.GetUser(User.GetID()))!;
            List<Pack> packs = new();
            foreach (var pack in user.Inventory.Packs)
            {
                packs.Add(await _tradeContext.Packs.Include(p => p.Cover).Include(c => c.Cards).Include(c => c.Rarities).ThenInclude(r => r.Rarity).Include(c => c.CardPool).ThenInclude(cp => cp.Card).FirstOrDefaultAsync(p => p.ID == pack.PackID));
            }
            return Ok(packs);
        }

        [HttpGet("in-inventory/{id}")]
        public async Task<ActionResult<IEnumerable<Pack>>> InInventory(Guid id)
        {
            User? user = await _tradeContext.Users.Include(u => u.Inventory).FirstOrDefaultAsync(u => u.ID == id);
            if (user is null)
            {
                return NotFound(Error.Create("User does not exist."));
            }
            if (user.Settings.Privacy != Models.Settings.InventoryPrivacy.Public)
            {
                return Unauthorized(Error.Create("User's inventory is private."));
            }
            List<Pack> packs = new();
            foreach (var pack in user.Inventory.Packs)
            {
                packs.Add(await _tradeContext.Packs.Include(p => p.Cover).Include(c => c.Cards).Include(c => c.Rarities).ThenInclude(r => r.Rarity).Include(c => c.CardPool).ThenInclude(cp => cp.Card).FirstOrDefaultAsync(p => p.ID == pack.PackID));
            }
            return Ok(packs);
        }

        [HttpGet("contains/{id}")]
        public async Task<IAsyncEnumerable<Pack>> GetPacksThatIncludeCard(Guid id)
        {
            Card? card = await _tradeContext.Cards.FindAsync(id);
            if (card is null || card.Public)
                return GetAllPacks();
            return _tradeContext.Packs.Include(p => p.Cover).Include(c => c.Cards).Include(c => c.Rarities).ThenInclude(r => r.Rarity).Include(c => c.CardPool).ThenInclude(cp => cp.Card).Where(c => c.CardPool.Any(r => r.CardID == id)).AsAsyncEnumerable();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pack>> GetPack(Guid id)
        {
            Pack? pack = await _tradeContext.Packs.Include(p => p.Cover).Include(c => c.Cards).Include(c => c.Rarities).ThenInclude(r => r.Rarity).Include(c => c.CardPool).ThenInclude(cp => cp.Card).FirstOrDefaultAsync(p => p.ID == id);
            if (pack is null)
            {
                return NotFound(Error.Create("Could not find pack."));
            }
            return Ok(pack);
        }

        [HttpPost]
        [Authorize(Scopes.CreatePack)]
        public async Task<ActionResult<Pack>> CreatePack([FromBody] CreatePackBody body)
        {
            Media? cover = await _tradeContext.Media.FindAsync(body.CoverID);
            if (cover is null)
            {
                return NotFound(Error.Create("Could not find cover."));
            }
            _logger.LogInformation("Creating new pack.");
            Pack pack = new Pack
            {
                Cover = cover,
                Name = body.Name,
                Theme = body.Theme,
                Value = body.Value,
                ID = Guid.NewGuid(),
                Description = body.Description,
                CardCount = body.CardCount ?? 5,
            };
            if (body.CardPool is not null)
            {
                await ProcessAdditionalCards(pack, body.CardPool);
            }
            if (body.Rarities is not null)
            {
                foreach (var rarityName in body.Rarities)
                {
                    Rarity? rarity = await _tradeContext.Rarities.FirstOrDefaultAsync(r => r.Name.ToLower() == rarityName.ToLower());
                    if (rarity is null)
                    {
                        continue;
                    }
                    pack.Rarities = new List<Rarity.Reference> { new Rarity.Reference { Rarity = rarity } };
                }
            }
            _tradeContext.Packs.Add(pack);
            await _tradeContext.SaveChangesAsync();
            return Ok(pack);
        }

        [HttpPatch]
        [Authorize(Scopes.ManagePack)]
        public async Task<ActionResult<Pack>> ModifyPack([FromBody] ModifyPackBody body)
        {
            Pack? pack = await _tradeContext.Packs.Include(p => p.Cover).Include(c => c.Cards).Include(c => c.Rarities).ThenInclude(r => r.Rarity).Include(c => c.CardPool).ThenInclude(cp => cp.Card).FirstOrDefaultAsync(p => p.ID == body.ID);
            if (pack is null)
            {
                return NotFound(Error.Create("Pack does not exist."));
            }
            _logger.LogInformation("Editing pack.");
            if (body.Description is not null)
            {
                pack.Description = body.Description;
            }
            if (body.Value is not null)
            {
                pack.Value = body.Value.Value < 0 ? null : body.Value.Value;
            }
            if (body.CardCount is not null)
            {
                pack.CardCount = body.CardCount.Value <= 0 ? 1 : body.CardCount.Value;
            }
            if (body.Theme is not null)
            {
                pack.Theme = body.Theme;
            }
            if (body.AdditionalCardPool is not null)
            {
                await ProcessAdditionalCards(pack, body.AdditionalCardPool);
            }
            if (body.AdditionalRarities is not null)
            {
                foreach (var rarityName in body.AdditionalRarities)
                {
                    Rarity? rarity = await _tradeContext.Rarities.FirstOrDefaultAsync(r => r.Name.ToLower() == rarityName.ToLower());
                    if (rarity is null)
                    {
                        continue;
                    }
                    pack.Rarities.Add(new Rarity.Reference { Rarity = rarity });
                }
            }
            if (body.DeleteCardPool is not null)
            {
                foreach (var toDelete in body.DeleteCardPool)
                {
                    var refCard = pack.CardPool.FirstOrDefault(cr => cr.CardID == toDelete);
                    if (refCard is not null)
                    {
                        pack.CardPool.Remove(refCard);
                    }
                }
            }
            if (body.DeleteRarities is not null)
            {
                foreach (var rarityName in body.DeleteRarities)
                {
                    Rarity? rarity = await _tradeContext.Rarities.FirstOrDefaultAsync(r => r.Name.ToLower() == rarityName.ToLower());
                    if (rarity is null)
                    {
                        continue;
                    }
                    Rarity.Reference? reference = pack.Rarities.FirstOrDefault(r => r.Rarity.ID == rarity.ID);
                    if (reference is not null)
                    {
                        pack.Rarities.Remove(reference);
                    }
                }
            }
            await _tradeContext.SaveChangesAsync();
            return Ok(pack);
        }

        private async Task ProcessAdditionalCards(Pack pack, ReferenceBody[] refCards)
        {
            var cards = (await _tradeContext.Cards.ToListAsync()).Where(c => refCards.Any(bc => bc.ID == c.ID));
            foreach (var card in cards)
            {
                var activeReference = refCards.First(c => c.ID == card.ID);
                var oldReference = pack.CardPool.FirstOrDefault(c => c.CardID == card.ID);
                if (oldReference is not null)
                {
                    if (activeReference.Boost is not null)
                    {
                        oldReference.Boost = activeReference.Boost.Value <= 0 ? null : activeReference.Boost.Value;
                    }
                    if (activeReference.Guaranteed is not null)
                    {
                        oldReference.Guaranteed = activeReference.Guaranteed.Value;
                    }
                    continue;
                }
                pack.CardPool.Add(new Card.Reference
                {
                    Card = card,
                    Boost = activeReference.Boost,
                    Guaranteed = activeReference.Guaranteed ?? false,
                });
            }
        }

        public class CreatePackBody
        {
            public string Name { get; set; } = null!;
            public string Description { get; set; } = null!;
            public Guid CoverID { get; set; }
            public float? Value { get; set; }
            public int? CardCount { get; set; }
            public ColorTheme Theme { get; set; } = null!;

            public string[]? Rarities { get; set; }
            public ReferenceBody[]? CardPool { get; set; }
        }

        public class ModifyPackBody
        {
            public Guid ID { get; set; }
            public string? Description { get; set; }
            public float? Value { get; set; }
            public int? CardCount { get; set; }
            public ColorTheme? Theme { get; set; }

            public string[]? AdditionalRarities { get; set; }
            public ReferenceBody[]? AdditionalCardPool { get; set; }
            public string[]? DeleteRarities { get; set; }
            public Guid[]? DeleteCardPool { get; set; }
        }

        public class ReferenceBody
        {
            public Guid ID { get; set; }
            public float? Boost { get; set; }
            public bool? Guaranteed { get; set; }
        }
    }
}