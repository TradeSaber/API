using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;

        public InventoryController(IAuthService authService, TradeContext tradeContext)
        {
            _authService = authService;
            _tradeContext = tradeContext;
        }

        [Authorize]
        [HttpGet("@me")]
        public async Task<ActionResult<Inventory>> GetSelfInventory()
        {
            User user = (await _authService.GetUser(User.GetID()))!;
            return Ok(user.Inventory);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetUsersInventory(Guid id)
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
            return Ok(user.Inventory);
        }
    }
}