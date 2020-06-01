using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [Route("cards")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CardDispatcher _cardDispatcher;

        public CardController(UserService user, CardDispatcher dispatcher)
        {
            _userService = user;
            _cardDispatcher = dispatcher;
        }

        [HttpGet("test/rolltest")]
        public IActionResult RollTest()
        {
            
            return Ok(_cardDispatcher.RollDeckRandom(5));
        }

        [HttpGet("{id}")]
        public IActionResult GetCard(string id)
        {
            Card card = _cardDispatcher.GetCard(id);
            if (card == null)
            {
                return NotFound();
            }
            return Ok(card);
        }

        [HttpGet("series/{id}")]
        public IActionResult GetCardsFromSeries(string id)
        {
            return Ok(_cardDispatcher.GetCardsFromSeries(id));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CardUpload body)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();

            long size = body.Cover.Length;

            if (size > 15000000 || size == 0)
                return BadRequest();

            Card card = body.Card;
            _cardDispatcher.Create(card);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                string newPath = path + "/" + card.Id + Path.GetExtension(body.Cover.FileName);
                using StreamReader sr = new StreamReader(body.Cover.OpenReadStream());
                using Stream stream = System.IO.File.Create(newPath);
                await body.Cover.CopyToAsync(stream);

                card.CoverURL = newPath;
                _cardDispatcher.Update(card);

                return Ok(card);
            }
            catch
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

            Card card = _cardDispatcher.GetCard(id);
            if (card == null)
                return NotFound();

            if (size > 15000000 || size == 0)
                return BadRequest();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                string newPath = path + "/" + card.Id + Path.GetExtension(file.FileName);
                using StreamReader sr = new StreamReader(file.OpenReadStream());
                using Stream stream = System.IO.File.Create(newPath);
                await file.CopyToAsync(stream);

                card.CoverURL = newPath;
                _cardDispatcher.Update(card);

                return Ok(card);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult UpdateCard(string id, [FromBody] JsonPatchDocument<Card> card)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Card c = _cardDispatcher.GetCard(id);
            if (c != null)
            {
                card.ApplyTo(c);
                _cardDispatcher.Update(c);
                return Ok(c);
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteCard(string id)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Card card = _cardDispatcher.GetCard(id);
            if (card != null)
            {
                Pack[] guarantees = _cardDispatcher.PacksContainingGuaranteedCard(card.Id);
                Pack[] locked = _cardDispatcher.PacksContainingLockedCard(card.Id);
                for (int i = 0; i < guarantees.Length; i++)
                {
                    Pack pack = guarantees[i];
                    pack.GuaranteedCards.RemoveAll(x => x == card.Id);
                    _cardDispatcher.Update(pack);
                }
                for (int i = 0; i < locked.Length; i++)
                {
                    Pack pack = locked[i];
                    pack.LockedCardPool.RemoveAll(x => x.Id == card.Id);
                    _cardDispatcher.Update(pack);
                }
            }
            return NoContent();
        }

        public class CardUpload
        {
            public Card Card { get; set; }
            public IFormFile Cover { get; set; }
        }

        [HttpGet("test/create")]
        public IActionResult CreateTest()
        {
            Series series = new Series
            {
                Name = "The BSMG Staff",
                Description = "The people who started it all. This collection consists of the Beat Saber Modding Group staff members"
            };

            _cardDispatcher.Create(series);

            Card reaxt = new Card
            {
                Locked = false,
                Rarity = Rarity.Legendary,
                BaseProbability = 0.00025,
                Description = "The TV Head himself, Reaxt.",
                Master = "Reaxt",
                Name = "Reaxt",
                MaxPrints = -1,
                Series = series.Id
            };

            Card elliottate = new Card
            {
                Locked = false,
                Rarity = Rarity.Rare,
                BaseProbability = 0.001,
                Description = "The man with connections everywhere, it's Elliott! Seriously... this guy knows everyone!",
                Master = "Elliottate",
                Name = "Elliottate",
                MaxPrints = -1,
                Series = series.Id
            };

            Card assistant = new Card
            {
                Locked = false,
                Rarity = Rarity.Rare,
                BaseProbability = 0.001,
                Description = "Everyone's favorite loli, Assistant!",
                Master = "Assistant",
                Name = "Assistant",
                MaxPrints = -1,
                Series = series.Id
            };

            Card umbranox = new Card
            {
                Locked = false,
                Rarity = Rarity.Uncommon,
                BaseProbability = 0.005,
                Description = "Panda!",
                Master = "Umbranox",
                Name = "Umbranox",
                MaxPrints = -1,
                Series = series.Id
            };

            Card williums = new Card
            {
                Locked = false,
                Rarity = Rarity.Uncommon,
                BaseProbability = 0.005,
                Description = "Is gay",
                Master = "williums",
                Name = "williums",
                MaxPrints = -1,
                Series = series.Id
            };

            Card megalon = new Card
            {
                Locked = false,
                Rarity = Rarity.Uncommon,
                BaseProbability = 0.005,
                Description = "His soothing ASMR voice will make anyone relax",
                Master = "Megalon",
                Name = "Megalon",
                MaxPrints = -1,
                Series = series.Id
            };

            Card bobbie = new Card
            {
                Locked = false,
                Rarity = Rarity.Common,
                BaseProbability = 0.05,
                Description = "Stop. Him. He. Is. Too. Powerful.",
                Master = "Bobbie",
                Name = "Bobbie",
                MaxPrints = -1,
                Series = series.Id
            };

            Card steven = new Card
            {
                Locked = false,
                Rarity = Rarity.Common,
                BaseProbability = 0.05,
                Description = "Lucina Lover",
                Master = "Steven",
                Name = "Steven",
                MaxPrints = -1,
                Series = series.Id
            };

            Card melopod = new Card
            {
                Locked = false,
                Rarity = Rarity.Common,
                BaseProbability = 0.05,
                Description = "",
                Master = "Melopod",
                Name = "Melopod",
                MaxPrints = -1,
                Series = series.Id
            };

            Card orangew = new Card
            {
                Locked = false,
                Rarity = Rarity.Common,
                BaseProbability = 0.05,
                Description = "",
                Master = "OrangeW",
                Name = "OrangeW",
                MaxPrints = -1,
                Series = series.Id
            };

            _cardDispatcher.Create(reaxt);
            _cardDispatcher.Create(assistant);
            _cardDispatcher.Create(elliottate);
            _cardDispatcher.Create(umbranox);
            _cardDispatcher.Create(williums);
            _cardDispatcher.Create(megalon);
            _cardDispatcher.Create(bobbie);
            _cardDispatcher.Create(steven);
            _cardDispatcher.Create(melopod);
            _cardDispatcher.Create(orangew);

            Series modders = new Series
            {
                Name = "Modders I",
                Description = "The first modders series!"
            };

            _cardDispatcher.Create(modders);

            Card danike = new Card
            {
                Name = "DaNike",
                Master = "DaNike",
                BaseProbability = .00025,
                Description = "The creator of BSIPA. His brain is so massive",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Legendary,
                Series = modders.Id
            };
            _cardDispatcher.Create(danike);

            Card andru = new Card
            {
                Name = "Andruzzzhka",
                Master = "Andruzzzhka",
                BaseProbability = .001,
                Description = "The UI Wizard",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Rare,
                Series = modders.Id
            };
            _cardDispatcher.Create(andru);

            Card nalulululuna = new Card
            {
                Name = "nalulululuna",
                Master = "nalulululuna",
                BaseProbability = .001,
                Description = "nalulululululululululululululululululululululuna",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Rare,
                Series = modders.Id
            };
            _cardDispatcher.Create(nalulululuna);

            Card shoko = new Card
            {
                Name = "Skoko84",
                Master = "Skoko84",
                BaseProbability = .005,
                Description = "The creator of FightSabers",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Uncommon,
                Series = modders.Id
            };
            _cardDispatcher.Create(shoko);

            Card brian = new Card
            {
                Name = "brian",
                Master = "brian",
                BaseProbability = .005,
                Description = "Give him pizza",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Uncommon,
                Series = modders.Id
            };
            _cardDispatcher.Create(brian);

            Card chris = new Card
            {
                Name = "chris",
                Master = "chris",
                BaseProbability = .005,
                Description = "chris",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Uncommon,
                Series = modders.Id
            };
            _cardDispatcher.Create(chris);

            Card aeroluna = new Card
            {
                Name = "Aeroluna",
                Master = "Aeroluna",
                BaseProbability = .05,
                Description = "Closet Furry A",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Common,
                Series = modders.Id
            };
            _cardDispatcher.Create(aeroluna);

            Card rabbit = new Card
            {
                Name = "Rabbit",
                Master = "Rabbit",
                BaseProbability = .05,
                Description = "Closet Furry B",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Common,
                Series = modders.Id
            };
            _cardDispatcher.Create(rabbit);

            Card opl = new Card
            {
                Name = "opl",
                Master = "opl",
                BaseProbability = .05,
                Description = "opl",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Common,
                Series = modders.Id
            };
            _cardDispatcher.Create(opl);

            Card moon = new Card
            {
                Name = "Moon",
                Master = "Moon",
                BaseProbability = .05,
                Description = "Moon",
                Locked = false,
                MaxPrints = -1,
                Rarity = Rarity.Common,
                Series = modders.Id
            };
            _cardDispatcher.Create(moon);

            return Ok();
        }
    }
}