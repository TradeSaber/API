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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;

        public AdminController(ILogger<AdminController> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
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
    }
}