using TradeSaber.Models;
using TradeSaber.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Specialized;

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

        [HttpGet("test/create")]
        public IActionResult CreatePacks()
        {
            Pack generic = new Pack
            {
                Name = "TradeSaber",
                Description = "A generic TradeSaber card pack",
            };
            _cardDispatcher.Create(generic);

            Pack bsmg = new Pack
            {
                Name = "BSMG Staff",
                Description = "BSMG Staff Pack! Higher chance for getting BSMG cards!",
                LockedCardPool = new List<ProbabilityDatum>
                {
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e2e",
                        ProbabilityBoost = .00015
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e2f",
                        ProbabilityBoost = .00075
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e30",
                        ProbabilityBoost = .00075
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e31",
                        ProbabilityBoost = .0025
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e32",
                        ProbabilityBoost = .0025
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e33",
                        ProbabilityBoost = .0025
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e34",
                        ProbabilityBoost = .025
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e35",
                        ProbabilityBoost = .025
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e36",
                        ProbabilityBoost = .025
                    },
                    new ProbabilityDatum
                    {
                        Id = "5ed1e7beb3badf0c30049e37",
                        ProbabilityBoost = .025
                    }
                },
            };
            _cardDispatcher.Create(bsmg);

            return Ok();
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
            bool packExists = user.UnopenedPacks.Contains(packId);

            if (packExists)
            {
                user.UnopenedPacks.Remove(packId);
                _userService.Update(user);
                var pack = _cardDispatcher.GetPack(packId);
                Card[] cards = _cardDispatcher.RollFromPack(pack);

                return Ok(cards);
            }
            return Ok();
        }
    }
}
