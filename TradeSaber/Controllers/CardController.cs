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
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class CardController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly HTIService _htiService;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly CardDispatcher _cardDispatcher;

        public CardController(ILogger<CardController> logger, HTIService htiService, IAuthService authService, TradeContext tradeContext, CardDispatcher cardDispatcher)
        {
            _logger = logger;
            _htiService = htiService;
            _authService = authService;
            _tradeContext = tradeContext;
            _cardDispatcher = cardDispatcher;
        }
        
        [HttpGet("random")]
        public async Task<ActionResult<IEnumerable<Card>>> Random()
        {
            var list = new List<Card>();
            for (int i = 0; i < 5; i++)
                list.Add(await _cardDispatcher.Roll());
            return list;
        }

        [HttpGet]
        public IAsyncEnumerable<Card> GetAllCards()
        {
            return _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).AsAsyncEnumerable();
        }

        [HttpGet("in-inventory/@me")]
        public async Task<ActionResult<IEnumerable<Card>>> InSelfInventory()
        {
            User user = (await _authService.GetUser(User.GetID()))!;
            List<Card> cards = new();
            foreach (var card in user.Inventory.Cards)
            {
                cards.Add(await _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).FirstOrDefaultAsync(c => c.ID == card.CardID));
            }
            return Ok(cards);
        }

        [HttpGet("in-inventory/{id}")]
        public async Task<ActionResult<IEnumerable<Card>>> InInventory(Guid id)
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
            List<Card> cards = new();
            foreach (var card in user.Inventory.Cards)
            {
                cards.Add(await _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).FirstOrDefaultAsync(c => c.ID == card.CardID));
            }
            return Ok(cards);
        }

        [HttpGet("exclusive-in-pack/{id}")]
        public async Task<ActionResult<IEnumerable<Card>>> GetPossibleCardsInPack(Guid id)
        {
            Pack? pack = await _tradeContext.Packs.FindAsync(id);
            if (pack is null)
            {
                return NotFound(Error.Create("Pack not found."));
            }
            List<Card> cards = new();
            var query = _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).Where(c => !c.Public).AsAsyncEnumerable();
            await foreach (Card card in query)
                if (pack.CardPool.Any(c => c.CardID == card.ID))
                    cards.Add(card);
            return Ok(cards);
        }

        [HttpGet("in-series/{id}")]
        public IAsyncEnumerable<Card> GetCardsInSeries(Guid id)
        {
            return _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).Where(c => c.Series.ID == id).AsAsyncEnumerable();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(Guid id)
        {
            Card? card = await _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).FirstOrDefaultAsync(c => c.ID == id);
            if (card is null)
            {
                return NotFound(Error.Create("Card not found."));
            }
            return Ok(card);
        }

        [HttpPost]
        [Authorize(Scopes.CreateCard)]
        public async Task<ActionResult<Card>> CreateCard([FromBody] CreateCardBody body)
        {
            User user = (await _authService.GetUser(User.GetID()))!;
            Series? series = await _tradeContext.Series.FindAsync(body.SeriesID);
            if (series is null)
            {
                return NotFound(Error.Create("Series does not exist."));
            }
            Rarity? rarity = await _tradeContext.Rarities.FirstOrDefaultAsync(r => r.Name.ToLower() == body.Rarity.ToLower());
            if (rarity is null)
            {
                return NotFound(Error.Create("Rarity does not exist."));
            }
            Media? baseMedia = await _tradeContext.Media.FindAsync(body.BaseID);
            if (baseMedia is null)
            {
                return NotFound(Error.Create("Base does not exist."));
            }
            _logger.LogInformation("Creating new card, {Name}.", body.Name);
            Media? faceMedia = await _htiService.Generate(body.Name, body.Description, rarity, series, baseMedia, user);
            if (faceMedia is null)
            {
                return BadRequest(Error.Create("Error occured while creating card face."));
            }
            Card card = new()
            {
                ID = Guid.NewGuid(),
                Name = body.Name,
                Description = body.Description,

                Rarity = rarity,
                Series = series,
                Base = baseMedia,
                Cover = faceMedia,

                Value = body.Value,
                Maximum = body.Maximum,
                Public = body.Public ?? true,
                Probability = body.Probability ?? 1
            };
            _tradeContext.Cards.Add(card);
            await _tradeContext.SaveChangesAsync();
            return Ok(card);
        }

        [HttpPatch]
        [Authorize(Scopes.ManageCard)]
        public async Task<ActionResult<Card>> ModifyCard([FromBody] ModifyCardBody body)
        {
            Card? card = await _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).FirstOrDefaultAsync(c => c.ID == body.ID);
            if (card is null)
            {
                return NotFound(Error.Create("Can't modify nonexistant card."));
            }
            if (body.Public is not null)
            {
                card.Public = body.Public.Value;
            }
            if (body.Maximum is not null)
            {
                card.Maximum = body.Maximum.Value < 0 ? null : body.Maximum.Value;
            }
            if (body.Value is not null)
            {
                card.Value = body.Value.Value < 0 ? null : body.Value.Value;
            }
            if (body.Probability is not null)
            {
                card.Probability = body.Probability.Value;
            }
            await _tradeContext.SaveChangesAsync();
            return Ok(card);
        }

        public class CreateCardBody
        {
            public string Name { get; set; } = null!;
            public string Description { get; set; } = null!;
            
            public string Rarity { get; set; } = null!;
            public Guid SeriesID { get; set; }
            public Guid BaseID { get; set; }

            public bool? Public { get; set; }
            public int? Maximum { get; set; }
            public float? Value { get; set; }
            public float? Probability { get; set; }
        }

        public class ModifyCardBody
        {
            public Guid ID { get; set; }
            public bool? Public { get; set; }
            public int? Maximum { get; set; }
            public float? Value { get; set; }
            public float? Probability { get; set; }
        }
    }
}