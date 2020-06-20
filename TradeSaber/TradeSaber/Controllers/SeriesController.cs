using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TradeSaber.Models;
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [Route("series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CardDispatcher _cardDispatcher;

        public SeriesController(UserService user, CardDispatcher dispatcher)
        {
            _userService = user;
            _cardDispatcher = dispatcher;
        }

        [HttpGet("{id}")]
        public IActionResult GetSeries(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            Series series = _cardDispatcher.GetSeries(id);
            if (series == null)
            {
                return NotFound();
            }
            return Ok(series);
        }

        [HttpGet("all")]
        public IActionResult GetAllSeries()
        {
            return Ok(_cardDispatcher.GetAllSeries());
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateSeries([FromBody] Series series)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            _cardDispatcher.Create(series);
            return Ok(series);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult UpdateSeries(string id, [FromBody] JsonPatchDocument<Series> series)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Series s = _cardDispatcher.GetSeries(id);
            if (s != null)
            {
                series.ApplyTo(s);
                _cardDispatcher.Update(s);
                return Ok(s);
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteSeries(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Series series = _cardDispatcher.GetSeries(id);
            if (series != null)
            {
                Card[] associatedCards = _cardDispatcher.GetCardsFromSeries(series.Id);
                for (int i = 0; i < associatedCards.Length; i++)
                {
                    _cardDispatcher.Delete(associatedCards[i]);
                }
                _cardDispatcher.Delete(series);
                return NoContent();
            }
            return NotFound();
        }
    }
}
