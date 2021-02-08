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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly RewardService _rewardService;

        public AdminController(ILogger<AdminController> logger, IAuthService authService, TradeContext tradeContext, RewardService rewardService)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
            _rewardService = rewardService;
        }

        [HttpGet("scopes")]
        public IEnumerable<string> AllScopes()
        {
            return Scopes.AllScopes;
        }

        [HttpPost("role")]
        [Authorize(Scopes.CreateRole)]
        public async Task<ActionResult<Role>> CreateRole([FromBody] CreateRoleBody body)
        {
            if (string.IsNullOrWhiteSpace(body.Name))
            {
                return BadRequest(Error.Create("Invalid role name."));
            }
            _logger.LogInformation("Creating role.");
            Role? role = await _tradeContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == body.Name.ToLower());
            if (role is not null)
            {
                return BadRequest(Error.Create("Role already exists."));
            }
            role = new Role
            {
                Name = body.Name,
                ID = Guid.NewGuid(),
                Scopes = body.InitialScopes?.ToList() ?? new List<string>()
            };
            _tradeContext.Roles.Add(role);
            await _tradeContext.SaveChangesAsync();
            return Ok(role);
        }

        [HttpPatch("role")]
        [Authorize(Scopes.ManageRole)]
        public async Task<ActionResult<Role>> ModifyRole([FromBody] ModifyRoleBody body)
        {
            Role? role = await _tradeContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == body.Name.ToLower());
            if (role is null)
            {
                return NotFound(Error.Create("Role not found."));
            }
            _logger.LogInformation("Editing role scopes.");
            if (body.AddScopes is not null)
            {
                foreach (var scope in body.AddScopes)
                {
                    if (!role.Scopes.Contains(scope) && Scopes.AllScopes.Contains(scope))
                    {
                        role.Scopes.Add(scope);
                    }
                } 
            }
            if (body.DeleteScopes is not null)
            {
                foreach (var scope in body.DeleteScopes)
                {
                    if (role.Scopes.Contains(scope))
                    {
                        role.Scopes.Remove(scope);
                    }
                }
            }
            await _tradeContext.SaveChangesAsync();
            return Ok(role);
        }

        [HttpPost("user")]
        [Authorize(Scopes.ManageUser)]
        public async Task<ActionResult<User>> ManageUser([FromBody] ManageUserBody body)
        {
            User? user = await _tradeContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.ID == body.ID);
            if (user is null)
            {
                return NotFound(Error.Create("User does not exist."));
            }
            if (body.Role is not null)
            {
                _logger.LogInformation("Changing user role.");
                if (body.Role == "")
                {
                    user.Role = null;
                }
                else
                {
                    Role? role = await _tradeContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == body.Role.ToLower());
                    if (role is null)
                    {
                        return NotFound(Error.Create("Role does not exist."));
                    }
                    user.Role = role;
                }
                await _tradeContext.SaveChangesAsync();
            }
            return Ok(user);
        }

        [HttpPost("give")]
        [Authorize(Scopes.ManageUser)]
        public async Task<ActionResult> GiveUserStuff([FromBody] GiveBody body)
        {
            User? user = await _authService.GetUser(body.UserID);
            if (user is null)
            {
                return NotFound(Error.Create("User does not exist."));
            }
            if (body.XP is not null)
            {
                await _rewardService.AddXP(user, body.XP.Value);
            }
            if (body.Tir is not null)
            {
                await _rewardService.AddTir(user, body.Tir.Value);
            }
            if (body.Cards is not null)
            {
                foreach (var cardID in body.Cards)
                {
                    Card? card = await _tradeContext.Cards.Include(c => c.Cover).Include(c => c.Base).FirstOrDefaultAsync(c => c.ID == cardID);
                    if (card is not null)
                    {
                        user.Inventory.Cards.Add(new Card.TradeableReference { Card = card });
                    }
                }
            }
            if (body.Packs is not null)
            {
                foreach (var packID in body.Packs)
                {
                    Pack? pack = await _tradeContext.Packs.Include(p => p.Cover).FirstOrDefaultAsync(p => p.ID == packID);
                    if (pack is not null)
                    {
                        user.Inventory.Packs.Add(new Pack.Reference { Pack = pack });
                    }
                }
            }
            return NoContent();
        }

        public class CreateRoleBody
        {
            public string Name { get; set; } = null!;
            public string[]? InitialScopes { get; set; }
        }

        public class ModifyRoleBody
        {
            public string Name { get; set; } = null!;
            public string[]? AddScopes { get; set; }
            public string[]? DeleteScopes { get; set; }
        }

        public class ManageUserBody
        {
            public Guid ID { get; set; }
            public string? Role { get; set; }
        }

        public class GiveBody
        {
            public Guid UserID { get; set; }
            public float? XP { get; set; }
            public float? Tir { get; set; }
            public Guid[]? Cards { get; set; }
            public Guid[]? Packs { get; set; }
        }
    }
}