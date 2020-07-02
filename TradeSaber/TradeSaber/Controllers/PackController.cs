using System;
using System.IO;
using MongoDB.Bson;
using TradeSaber.Models;
using TradeSaber.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace TradeSaber.Controllers
{
    [Route("packs")]
    [ApiController]
    public class PackController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CardDispatcher _cardDispatcher;

        public PackController(UserService user, CardDispatcher dispatcher)
        {
            _userService = user;
            _cardDispatcher = dispatcher;
        }

        [HttpGet("{id}")]
        public IActionResult GetPack(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            Pack pack = _cardDispatcher.GetPack(id);
            if (pack == null)
            {
                return NotFound();
            }
            return Ok(pack);
        }

        [HttpGet("all")]
        public IActionResult GetAllPacks()
        {
            return Ok(_cardDispatcher.GetAllPacks());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadPack([FromForm] UploadPack body)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();

            long size = body.Cover.Length;

            if (size > 15000000 || size == 0)
                return BadRequest();

            List<ProbabilityDatum> probData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProbabilityDatum>>(body.ProbData);
            List<string> cardData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(body.CardData);
            List<Rarity> rarityData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Rarity>>(body.RarityData);

            Pack pack = new Pack
            {
                Name = body.Name,
                Description = body.Description,
                GuaranteedCards = cardData,
                GuaranteedRarities = rarityData,
                LockedCardPool = probData,
                Count = body.Count,
                Theme = body.Theme
            };
            _cardDispatcher.Create(pack);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                string newPath = path + "/" + pack.Id + Path.GetExtension(body.Cover.FileName);
                using StreamReader sr = new StreamReader(body.Cover.OpenReadStream());
                using Stream stream = System.IO.File.Create(newPath);
                await body.Cover.CopyToAsync(stream);

                pack.CoverURL = "/images/" + pack.Id + Path.GetExtension(body.Cover.FileName); ;
                _cardDispatcher.Update(pack);

                return Ok(pack);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPatch("image/{id}")]
        public async Task<IActionResult> UpdateImage(string id, [FromForm] IFormFile file)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();

            long size = file.Length;

            Pack pack = _cardDispatcher.GetPack(id);
            if (pack == null)
                return NotFound();

            if (size > 15000000 || size == 0)
                return BadRequest();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                string newPath = path + "/" + pack.Id + Path.GetExtension(file.FileName);
                using StreamReader sr = new StreamReader(file.OpenReadStream());
                using Stream stream = System.IO.File.Create(newPath);
                await file.CopyToAsync(stream);

                pack.CoverURL = "/images/" + pack.Id + Path.GetExtension(file.FileName);
                _cardDispatcher.Update(pack);

                return Ok(pack);
            }
            catch
            {
                return BadRequest();
            }
        } 

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult UpdatePack(string id, [FromBody] JsonPatchDocument<Pack> pack)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Pack p = _cardDispatcher.GetPack(id);
            if (p != null)
            {
                pack.ApplyTo(p);
                _cardDispatcher.Update(p);
                return Ok(p);
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeletePack(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Pack pack = _cardDispatcher.GetPack(id);
            if (pack != null)
            {
                User[] users = _userService.UsersWithPack(pack.Id);
                for (int i = 0; i < users.Length; i++)
                {
                    User u = users[i];
                    u.UnopenedPacks.RemoveAll(p => p == pack.Id);
                    _userService.Update(u);
                }
                _cardDispatcher.Delete(pack);
            }
            return NoContent();
        }

        public class PackUpload
        {
            public Pack Pack { get; set; }
            public IFormFile Cover { get; set; }
        }

        [Authorize]
        [HttpGet("request")]
        public IActionResult RequestNewPack()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("open")]
        public IActionResult OpenPack([FromHeader] string packId)
        {
            User user = _userService.UserFromContext(HttpContext);

            if (!ObjectId.TryParse(packId, out ObjectId _))
                return BadRequest();

            bool packExists = user.UnopenedPacks.Contains(packId);

            if (packExists)
            {
                user.UnopenedPacks.Remove(packId);
                _userService.Update(user);
                var pack = _cardDispatcher.GetPack(packId);
                Card[] cards = _cardDispatcher.RollFromPack(pack);
                var cardIds = cards.Select(c => c.Id);
                user.Inventory.AddRange(cardIds);
                _userService.Update(user);

                return Ok(cards);
            }
            return NotFound();
        }
    }
}
