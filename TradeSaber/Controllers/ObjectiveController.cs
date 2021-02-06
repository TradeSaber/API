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
    public class ObjectiveController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly RewardService _rewardService;

        public ObjectiveController(ILogger<ObjectiveController> logger, IAuthService authService, TradeContext tradeContext, RewardService rewardService)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
            _rewardService = rewardService;
        }

        [HttpGet]
        public IAsyncEnumerable<Objective> GetAllActiveObjectives()
        {
            return _tradeContext.Objectives.Include(o => o.Icon).Include(o => o.PackRewards).ThenInclude(pr => pr.Pack).Where(o => o.Active).AsAsyncEnumerable();
        }

        [HttpGet("full")]
        [Authorize(Scopes.ManageObjective)]
        public IAsyncEnumerable<Objective> GetAllObjectives()
        {
            return _tradeContext.Objectives.Include(o => o.Icon).Include(o => o.PackRewards).ThenInclude(pr => pr.Pack).AsAsyncEnumerable();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Objective>> GetObjective(Guid id)
        {
            Objective? objective = await _tradeContext.Objectives.Include(o => o.Icon).Include(o => o.PackRewards).ThenInclude(pr => pr.Pack).FirstOrDefaultAsync(o => o.ID == id);
            if (objective is null)
            {
                return NotFound(Error.Create("Objective not found."));
            }
            return Ok(objective);
        }

        [HttpGet("completed")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Objective>>> GetCompletedObjectivesForAuthorizedUser()
        {
            Guid id = User.GetID()!.Value;

            IEnumerable<Objective> objectives = await _tradeContext.Objectives
                .Include(o => o.Icon)
                .Include(o => o.PackRewards).ThenInclude(pr => pr.Pack)
                .Include(o => o.ObjectiveResults)
                .Where(o => o.ObjectiveResults
                .Any(or => or.Submitter.ID == id) && o.Active)
                .ToListAsync();

            return Ok(objectives);
        }

        [HttpPost]
        [Authorize(Scopes.CreateObjective)]
        public async Task<ActionResult<Objective>> CreateObjective([FromBody] CreateObjectiveBody body)
        {
            Media? icon = await _tradeContext.Media.FindAsync(body.IconID);
            if (icon is null)
            {
                return NotFound(Error.Create("Could not find icon."));
            }
            _logger.LogInformation("Creating new objective.");
            Objective objective = new Objective
            {
                Icon = icon,
                ID = Guid.NewGuid(),
                Subject = body.Subject,
                Template = body.Template,
                XPReward = body.XPReward,
                ObjectiveType = body.Type,
                TirReward = body.TirReward,
                Active = body.InitialState ?? false,
            };
            if (body.PackRewards is not null)
            {
                IEnumerable<Pack> packs = (await _tradeContext.Packs.ToListAsync()).Where(p => body.PackRewards.Contains(p.ID));
                foreach (var pack in packs)
                {
                    objective.PackRewards.Add(new Pack.Reference { Pack = pack });
                }
            }
            _tradeContext.Objectives.Add(objective);
            await _tradeContext.SaveChangesAsync();
            return Ok(objective);
        }

        [HttpPatch]
        [Authorize(Scopes.ManageObjective)]
        public async Task<ActionResult<Objective>> ModifyObjective([FromBody] ModifyObjectiveBody body)
        {
            Objective? objective = await _tradeContext.Objectives.Include(o => o.Icon).Include(o => o.PackRewards).ThenInclude(pr => pr.Pack).FirstOrDefaultAsync(o => o.ID == body.ID);
            if (objective is null)
            {
                return NotFound(Error.Create("Objective not found."));
            }
            _logger.LogInformation("Editing objective, {ID}", body.ID);
            if (body.XPReward is not null)
            {
                objective.XPReward = body.XPReward.Value <= 0 ? null : body.XPReward.Value;
            }
            if (body.TirReward is not null)
            {
                objective.TirReward = body.TirReward.Value <= 0 ? null : body.TirReward.Value;
            }
            if (body.NewState is not null)
            {
                objective.Active = body.NewState.Value;
            }
            if (body.AddPackRewards is not null)
            {
                IEnumerable<Pack> packs = (await _tradeContext.Packs.ToListAsync()).Where(p => body.AddPackRewards.Contains(p.ID));
                foreach (var pack in packs)
                {
                    objective.PackRewards.Add(new Pack.Reference { Pack = pack });
                }
            }
            if (body.DeletePackRewards is not null)
            {
                foreach (var reward in body.DeletePackRewards)
                {
                    Pack.Reference? reference = objective.PackRewards.FirstOrDefault(r => r.ID == reward);
                    if (reference is not null)
                    {
                        objective.PackRewards.Remove(reference);
                    }
                }
            }
            await _tradeContext.SaveChangesAsync();
            return Ok(objective);
        }

        [Authorize]
        [HttpPost("submit")]
        public async Task<ActionResult<Objective.Result>> SubmitObjective([FromBody] SubmitObjectiveBody body)
        {
            Objective? objective = await _tradeContext.Objectives.Include(o => o.Icon).Include(o => o.PackRewards).ThenInclude(pr => pr.Pack).FirstOrDefaultAsync(o => o.ID == body.ObjectiveID);
            if (objective is null)
            {
                return NotFound(Error.Create("Objective not found."));
            }
            if (!objective.Active)
            {
                return NotFound(Error.Create("Objective is not active."));
            }
            User submitter = (await _authService.GetUser(User.GetID()))!;

            if (objective.ObjectiveResults.Any(or => or.Submitter.ID == submitter.ID))
            {
                return BadRequest(Error.Create("Objective already submitted."));
            }

            float? modValue = null;
            const float maxModCombo = 1.18f;
            if (objective.ObjectiveType == Objective.Type.PlayLevel && body.Association != null)
            {
                if (objective.Subject!.Contains("max:"))
                {
                    string[] splitter = objective.Subject!.Split("|");
                    string? maxText = splitter.FirstOrDefault(s => s.StartsWith("max:"));
                    if (maxText is not null)
                    {
                        splitter = maxText.Split(":");
                        string? max = splitter[1];
                        if (float.TryParse(max, out float modMaxTrue))
                        {
                            if (body.Association >= modMaxTrue)
                            {
                                float clampedAssociation = Math.Clamp(body.Association.Value, modMaxTrue, maxModCombo);
                                modValue = clampedAssociation - modMaxTrue + 1;
                            }
                        }
                    }
                }
            }
            Objective.Result result = new Objective.Result
            {
                Modifier = modValue,
                Objective = objective,
                Submitter = submitter,
                Submitted = DateTime.UtcNow,
            };

            objective.ObjectiveResults.Add(result);
            if (objective.XPReward is not null)
            {
                await _rewardService.AddXP(submitter, modValue == null ? objective.XPReward.Value : objective.XPReward.Value * modValue.Value);
            }
            if (objective.TirReward is not null)
            {
                await _rewardService.AddTir(submitter, modValue == null ? objective.TirReward.Value : objective.TirReward.Value * modValue.Value);
            }
            await _tradeContext.SaveChangesAsync();
            return Ok(result);
        }

        public class CreateObjectiveBody
        {
            public string Template { get; set; } = null!;
            public float? XPReward { get; set; }
            public float? TirReward { get; set; }
            public Guid[]? PackRewards { get; set; }
            public Objective.Type Type { get; set; }
            public string? Subject { get; set; } = null!;
            public bool? InitialState { get; set; }
            public Guid IconID { get; set; }
        }

        public class ModifyObjectiveBody
        {
            public Guid ID { get; set; }
            public float? XPReward { get; set; }
            public float? TirReward { get; set; }
            public bool? NewState { get; set; }
            public Guid[]? AddPackRewards { get; set; }
            public Guid[]? DeletePackRewards { get; set; }
        }

        public class SubmitObjectiveBody
        {
            public Guid ObjectiveID { get; set; }
            public float? Association { get; set; }
        }
    }
}