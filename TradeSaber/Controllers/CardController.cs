using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public CardController(ILogger<CardController> logger, HTIService htiService, IAuthService authService, TradeContext tradeContext)
        {
            _logger = logger;
            _htiService = htiService;
            _authService = authService;
            _tradeContext = tradeContext;
        }

        [HttpGet]
        public IAsyncEnumerable<Card> GetAllCards()
        {
            return _tradeContext.Cards.Include(c => c.Rarity).Include(c => c.Series).Include(c => c.Base).Include(c => c.Cover).AsAsyncEnumerable();
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
            Card card = new Card
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
    }
}