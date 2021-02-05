using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class MutationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;

        public MutationController(ILogger logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }

        [HttpGet]
        public IAsyncEnumerable<Mutation> AllMutations()
        {
            return _tradeContext.Mutations.Include(m => m.Cards).Include(m => m.Series).AsAsyncEnumerable();
        }

        [HttpPost]
        [Authorize(Scopes.CreateMutation)]
        public async Task<ActionResult<Mutation>> CreateMutation([FromBody] CreateMutationBody body)
        {
            Mutation? mutation = await _tradeContext.Mutations.FirstOrDefaultAsync(m => m.Name.ToLower() == body.Name.ToString());
            if (mutation is not null)
            {
                return BadRequest(Error.Create("Mutation already exists."));
            }
            _logger.LogInformation("Creating new mutation.");
            mutation = new Mutation
            {
                Name = body.Name,
                ID = Guid.NewGuid(),
                Active = body.InitialState,
                GlobalXPBoost = body.XPBoost,
                GlobalTirBoost = body.TirBoost,
            };
            if (body.Cards is not null)
            {
                await ProcessAdditionalCards(mutation, body.Cards);
            }
            if (body.Series is not null)
            {
                await ProcessAdditionalSeries(mutation, body.Series);
            }
            _tradeContext.Mutations.Add(mutation);
            await _tradeContext.SaveChangesAsync();
            return Ok(mutation);
        }

        [HttpPatch]
        [Authorize(Scopes.ManageMutation)]
        public async Task<ActionResult<Mutation>> ManageMutation([FromBody] ManageMutationBody body)
        {
            Mutation? mutation = await _tradeContext.Mutations
                .Include(m => m.Cards).ThenInclude(cr => cr.Card)
                .Include(m => m.Series).ThenInclude(cr => cr.Series)
                .FirstOrDefaultAsync(m => m.ID == body.ID);

            if (mutation is null)
            {
                return NotFound(Error.Create("Mutation does not exist."));
            }
            if (body.State is not null)
            {
                mutation.Active = body.State.Value;
            }
            if (body.TirBoost is not null)
            {
                mutation.GlobalTirBoost = body.TirBoost.Value <= 0 ? null : body.TirBoost.Value;
            }
            if (body.XPBoost is not null)
            {
                mutation.GlobalTirBoost = body.XPBoost.Value <= 0 ? null : body.XPBoost.Value;
            }
            if (body.AdditionalCards is not null)
            {
                await ProcessAdditionalCards(mutation, body.AdditionalCards);
            }
            if (body.AdditionalSeries is not null)
            {
                await ProcessAdditionalSeries(mutation, body.AdditionalSeries);
            }
            if (body.DeleteCards is not null)
            {
                foreach (var toDelete in body.DeleteCards)
                {
                    var refCard = mutation.Cards.FirstOrDefault(cr => cr.CardID == toDelete);
                    if (refCard is not null)
                    {
                        mutation.Cards.Remove(refCard);
                    }
                }
            }
            if (body.DeleteSeries is not null)
            {
                foreach (var toDelete in body.DeleteSeries)
                {
                    var refSeries = mutation.Series.FirstOrDefault(sr => sr.SeriesID == toDelete);
                    if (refSeries is not null)
                    {
                        mutation.Series.Remove(refSeries);
                    }
                }
            }
            await _tradeContext.SaveChangesAsync();
            return Ok(mutation);
        }

        [HttpDelete("{id}")]
        [Authorize(Scopes.DeleteMutation)]
        public async Task<ActionResult> DeleteMutation(Guid id)
        {
            Mutation? mutation = await _tradeContext.Mutations.FirstOrDefaultAsync(m => m.ID == id);
            if (mutation is null)
            {
                return NotFound("Mutation does not exist.");
            }
            _tradeContext.Mutations.Remove(mutation);
            await _tradeContext.SaveChangesAsync();
            return NoContent();
        }

        private async Task ProcessAdditionalCards(Mutation mutation, ReferenceBody[] refCards)
        {
            var cards = (await _tradeContext.Cards.ToListAsync()).Where(c => refCards.Any(bc => bc.ID == c.ID));
            foreach (var card in cards)
            {
                var activeReference = refCards.First(c => c.ID == card.ID);
                mutation.Cards.Add(new Card.Reference
                {
                    Card = card,
                    Boost = activeReference.Boost,
                    Guaranteed = activeReference.Guaranteed ?? false,
                });
            }
        }

        private async Task ProcessAdditionalSeries(Mutation mutation, ReferenceBody[] refSeries)
        {
            var series = (await _tradeContext.Series.ToListAsync()).Where(s => refSeries.Any(bs => bs.ID == s.ID));
            foreach (var seri in series)
            {
                var activeReference = refSeries.First(s => s.ID == seri.ID);
                mutation.Series.Add(new Series.Reference
                {
                    Series = seri,
                    Boost = activeReference.Boost,
                });
            }
        }

        public class CreateMutationBody
        {
            public string Name { get; set; } = null!;
            public bool InitialState { get; set; }
            public float? TirBoost { get; set; }
            public float? XPBoost { get; set; }
            public ReferenceBody[]? Series { get; set; }
            public ReferenceBody[]? Cards { get; set; }
        }

        public class ManageMutationBody
        {
            public Guid ID { get; set; }
            public bool? State { get; set; }
            public float? TirBoost { get; set; }
            public float? XPBoost { get; set; }
            public ReferenceBody[]? AdditionalSeries { get; set; }
            public ReferenceBody[]? AdditionalCards { get; set; }
            public Guid[]? DeleteSeries { get; set; }
            public Guid[]? DeleteCards { get; set; }
        }

        public class ReferenceBody
        {
            public Guid ID { get; set; }
            public float? Boost { get; set; }
            public bool? Guaranteed { get; set; }
        }
    }
}