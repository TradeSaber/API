using TradeSaber.Models;
using TradeSaber.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TradeSaber.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("self")]
        public IActionResult GetSelf()
        {
            User user = _userService.UserFromContext(HttpContext);
            return Ok(user);            
        } 
    }
}